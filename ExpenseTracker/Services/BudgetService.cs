using ExpenseTracker.Data;
using ExpenseTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionService _transactionService;

        public BudgetService(ApplicationDbContext context, ITransactionService transactionService)
        {
            _context = context;
            _transactionService = transactionService;
        }

        // Basic CRUD operations
        public async Task<List<Budget>> GetBudgetsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();

            // Calculate spent amount for each budget
            foreach (var budget in budgets)
            {
                budget.CurrentSpending = await CalculateSpentAmountAsync(budget.Id, startDate, endDate);
            }

            return budgets;
        }

        public async Task<Budget> GetBudgetAsync(int id)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Budget> GetBudgetWithSpendingAsync(int id)
        {
            var budget = await GetBudgetAsync(id);
            if (budget != null)
            {
                budget.CurrentSpending = await CalculateSpentAmountAsync(budget.Id);
            }
            return budget;
        }

        public async Task CreateBudgetAsync(Budget budget)
        {
            // Set default dates based on period
            SetDefaultDates(budget);

            // Calculate next reset date
            budget.NextResetDate = CalculateNextResetDate(budget, budget.StartDate);

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBudgetAsync(Budget budget)
        {
            // Set default dates based on period
            SetDefaultDates(budget);

            // Recalculate next reset date
            budget.NextResetDate = CalculateNextResetDate(budget, budget.StartDate);

            budget.ModifiedDate = DateTime.Now;
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBudgetAsync(int id)
        {
            var budget = await GetBudgetAsync(id);
            if (budget != null)
            {
                // Delete associated notifications first
                var notifications = await _context.BudgetNotifications
                    .Where(n => n.BudgetId == id)
                    .ToListAsync();

                if (notifications.Any())
                {
                    _context.BudgetNotifications.RemoveRange(notifications);
                }

                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();
            }
        }

        // Budget tracking and calculation
        public async Task<List<Budget>> GetActiveBudgetsAsync()
        {
            var now = DateTime.Now;
            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.StartDate <= now && (b.EndDate == null || b.EndDate >= now))
                .OrderBy(b => b.Name)
                .ToListAsync();

            // Calculate spent amount for each budget
            foreach (var budget in budgets)
            {
                budget.CurrentSpending = await CalculateSpentAmountAsync(budget.Id);
            }

            return budgets;
        }

        public async Task<decimal> CalculateSpentAmountAsync(int budgetId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var budget = await GetBudgetAsync(budgetId);
            if (budget == null) return 0;

            // Determine date range
            DateTime actualStartDate = startDate ?? budget.StartDate;
            DateTime actualEndDate = endDate ?? (budget.EndDate ?? DateTime.Now);

            // Get transactions for the date range
            var transactions = await _transactionService.GetTransactionsAsync(
                startDate: actualStartDate,
                endDate: actualEndDate);

            // Filter by category if specified
            if (budget.CategoryId.HasValue)
            {
                transactions = transactions
                    .Where(t => t.CategoryId == budget.CategoryId.Value && t.Type == TransactionType.Expense)
                    .ToList();
            }
            else
            {
                // If no category specified, sum all expenses
                transactions = transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .ToList();
            }

            return transactions.Sum(t => t.Amount);
        }

        // Enhanced features for tracking
        public async Task CheckBudgetsAgainstTransaction(Transaction transaction)
        {
            if (transaction.Type != TransactionType.Expense)
                return;

            var activeBudgets = await GetActiveBudgetsAsync();

            foreach (var budget in activeBudgets)
            {
                // Check if transaction falls within budget period
                if (!IsTransactionInBudgetPeriod(transaction, budget))
                    continue;

                // Check if budget applies to this transaction
                if (!DoesBudgetApplyToTransaction(transaction, budget))
                    continue;

                // Update budget spending
                budget.CurrentSpending = await CalculateSpentAmountAsync(budget.Id);

                // Check if budget is exceeded or in warning
                await CheckBudgetStatus(budget);
            }
        }

        public async Task ResetExpiredBudgets()
        {
            var now = DateTime.Now;
            var budgetsToReset = await _context.Budgets
                .Where(b => b.NextResetDate != null && b.NextResetDate <= now)
                .ToListAsync();

            foreach (var budget in budgetsToReset)
            {
                budget.LastResetDate = now;
                budget.NextResetDate = CalculateNextResetDate(budget, now);
                budget.ModifiedDate = now;

                await _context.SaveChangesAsync();

                // Create reset notification
                var notification = new BudgetNotification
                {
                    BudgetId = budget.Id,
                    Message = $"Budget '{budget.Name}' has been reset for the new period.",
                    Type = NotificationType.BudgetReset,
                    CreatedDate = now
                };

                _context.BudgetNotifications.Add(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Budget>> GetBudgetsRequiringAttentionAsync()
        {
            var budgets = await GetActiveBudgetsAsync();
            return budgets
                .Where(b => b.Status == BudgetStatusType.Exceeded || b.Status == BudgetStatusType.Warning)
                .OrderByDescending(b => b.Status)
                .ThenByDescending(b => b.PercentageUsed)
                .ToList();
        }

        public async Task<List<BudgetNotification>> GetBudgetNotificationsAsync(int? budgetId = null, bool unreadOnly = false)
        {
            var query = _context.BudgetNotifications
                .Include(n => n.Budget)
                .OrderByDescending(n => n.CreatedDate)
                .AsQueryable();

            if (budgetId.HasValue)
            {
                query = query.Where(n => n.BudgetId == budgetId.Value);
            }

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            return await query.ToListAsync();
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _context.BudgetNotifications
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllNotificationsAsReadAsync()
        {
            var unreadNotifications = await _context.BudgetNotifications
                .Where(n => !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        // For reports
        public async Task<List<Budget>> GetBudgetsWithSpendingAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .ToListAsync();

            foreach (var budget in budgets)
            {
                budget.CurrentSpending = await CalculateSpentAmountAsync(budget.Id, startDate, endDate);
            }

            return budgets;
        }

        // Summary and analysis
        public async Task<BudgetSummary> GetBudgetSummaryAsync()
        {
            var budgets = await GetActiveBudgetsAsync();

            return new BudgetSummary
            {
                TotalBudgets = budgets.Count,
                ActiveBudgets = budgets.Count,
                OnTrackBudgets = budgets.Count(b => b.Status == BudgetStatusType.OnTrack),
                WarningBudgets = budgets.Count(b => b.Status == BudgetStatusType.Warning),
                ExceededBudgets = budgets.Count(b => b.Status == BudgetStatusType.Exceeded),
                TotalBudgetedAmount = budgets.Sum(b => b.Amount),
                TotalCurrentSpending = budgets.Sum(b => b.CurrentSpending)
            };
        }

        public async Task<List<BudgetNotification>> GetRecentNotificationsAsync(int count = 10)
        {
            return await _context.BudgetNotifications
                .Include(n => n.Budget)
                .OrderByDescending(n => n.CreatedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Budget>> GetBudgetsWithProgressAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            // Get budgets with their spending calculated
            var budgets = await GetBudgetsWithSpendingAsync(startDate, endDate);

            // Additional logic if needed - could filter or sort by progress
            // For example, you might want to sort by percentage used or only show budgets with some progress
            return budgets
                .OrderByDescending(b => b.PercentageUsed)
                .ThenBy(b => b.Name)
                .ToList();
        }

        // Helper methods
        private bool IsTransactionInBudgetPeriod(Transaction transaction, Budget budget)
        {
            return transaction.Date >= budget.StartDate &&
                   (budget.EndDate == null || transaction.Date <= budget.EndDate);
        }

        private bool DoesBudgetApplyToTransaction(Transaction transaction, Budget budget)
        {
            // If budget has no category, it applies to all expenses
            if (!budget.CategoryId.HasValue)
                return true;

            // Otherwise, check if transaction matches the category
            return transaction.CategoryId == budget.CategoryId.Value;
        }

        private async Task CheckBudgetStatus(Budget budget)
        {
            // Check if budget is exceeded
            if (budget.CurrentSpending > budget.Amount && budget.NotifyOnExceed)
            {
                await CreateBudgetNotification(
                    budget,
                    $"Budget '{budget.Name}' has been exceeded! Spent: {budget.CurrentSpending:C}, Limit: {budget.Amount:C}",
                    NotificationType.BudgetExceeded
                );
            }
            // Check if budget is in warning zone
            else if (budget.PercentageUsed >= budget.WarningThreshold && budget.NotifyOnWarning)
            {
                await CreateBudgetNotification(
                    budget,
                    $"Budget '{budget.Name}' is {budget.PercentageUsed:F1}% used. Remaining: {budget.RemainingAmount:C}",
                    NotificationType.BudgetWarning
                );
            }
        }

        private async Task CreateBudgetNotification(Budget budget, string message, NotificationType type)
        {
            // Check if similar notification already exists recently (within last 24 hours)
            var recentNotification = await _context.BudgetNotifications
                .Where(n => n.BudgetId == budget.Id &&
                           n.Type == type &&
                           n.CreatedDate >= DateTime.Now.AddDays(-1))
                .FirstOrDefaultAsync();

            if (recentNotification == null)
            {
                var notification = new BudgetNotification
                {
                    BudgetId = budget.Id,
                    Message = message,
                    Type = type,
                    CreatedDate = DateTime.Now
                };

                _context.BudgetNotifications.Add(notification);
                await _context.SaveChangesAsync();
            }
        }

        private void SetDefaultDates(Budget budget)
        {
            var now = DateTime.Now;

            switch (budget.Period)
            {
                case BudgetPeriod.Daily:
                    budget.StartDate = budget.StartDate.Date;
                    budget.EndDate = budget.StartDate.AddDays(1).AddSeconds(-1);
                    break;

                case BudgetPeriod.Weekly:
                    // Start from Monday of the selected week
                    var daysToMonday = ((int)budget.StartDate.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                    budget.StartDate = budget.StartDate.Date.AddDays(-daysToMonday);
                    budget.EndDate = budget.StartDate.AddDays(7).AddSeconds(-1);
                    break;

                case BudgetPeriod.Monthly:
                    budget.StartDate = new DateTime(budget.StartDate.Year, budget.StartDate.Month, 1);
                    budget.EndDate = budget.StartDate.AddMonths(1).AddSeconds(-1);
                    break;

                case BudgetPeriod.Quarterly:
                    var quarter = (budget.StartDate.Month - 1) / 3;
                    budget.StartDate = new DateTime(budget.StartDate.Year, quarter * 3 + 1, 1);
                    budget.EndDate = budget.StartDate.AddMonths(3).AddSeconds(-1);
                    break;

                case BudgetPeriod.Yearly:
                    budget.StartDate = new DateTime(budget.StartDate.Year, 1, 1);
                    budget.EndDate = new DateTime(budget.StartDate.Year, 12, 31, 23, 59, 59);
                    break;

                case BudgetPeriod.Custom:
                    // Use the dates provided by the user
                    if (budget.EndDate == null)
                    {
                        budget.EndDate = budget.StartDate.AddMonths(1).AddSeconds(-1);
                    }
                    break;
            }
        }

        private DateTime? CalculateNextResetDate(Budget budget, DateTime currentDate)
        {
            if (budget.EndDate.HasValue && budget.EndDate > currentDate)
            {
                return budget.EndDate.Value;
            }

            return budget.Period switch
            {
                BudgetPeriod.Daily => currentDate.AddDays(1),
                BudgetPeriod.Weekly => currentDate.AddDays(7),
                BudgetPeriod.Monthly => currentDate.AddMonths(1),
                BudgetPeriod.Quarterly => currentDate.AddMonths(3),
                BudgetPeriod.Yearly => currentDate.AddYears(1),
                BudgetPeriod.Custom => null, // Custom budgets don't auto-reset
                _ => null
            };
        }
    }

    // Added missing BudgetSummary class
    public class BudgetSummary
    {
        public int TotalBudgets { get; set; }
        public int ActiveBudgets { get; set; }
        public int OnTrackBudgets { get; set; }
        public int WarningBudgets { get; set; }
        public int ExceededBudgets { get; set; }
        public decimal TotalBudgetedAmount { get; set; }
        public decimal TotalCurrentSpending { get; set; }
        public decimal TotalRemainingAmount => TotalBudgetedAmount - TotalCurrentSpending;
        public decimal OverallBudgetUsage => TotalBudgetedAmount > 0 ? (TotalCurrentSpending / TotalBudgetedAmount) * 100 : 0;

        public List<BudgetStatusSummary> BudgetStatuses { get; set; } = new();
    }
}