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

        public async Task<List<Budget>> GetBudgetsAsync()
        {
            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync();

            // Calculate spent amount for each budget
            foreach (var budget in budgets)
            {
                budget.SpentAmount = await CalculateSpentAmountAsync(budget.Id);
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
                budget.SpentAmount = await CalculateSpentAmountAsync(budget.Id);
            }
            return budget;
        }

        public async Task CreateBudgetAsync(Budget budget)
        {
            // Set default dates based on period
            SetDefaultDates(budget);

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBudgetAsync(Budget budget)
        {
            // Set default dates based on period
            SetDefaultDates(budget);

            budget.ModifiedDate = DateTime.Now;
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBudgetAsync(int id)
        {
            var budget = await GetBudgetAsync(id);
            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();
            }
        }

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
                budget.SpentAmount = await CalculateSpentAmountAsync(budget.Id);
            }

            return budgets;
        }

        public async Task<decimal> CalculateSpentAmountAsync(int budgetId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var budget = await GetBudgetAsync(budgetId);
            if (budget == null) return 0;

            // Determine date range based on budget period
            DateTime actualStartDate = startDate ?? GetPeriodStartDate(budget);
            DateTime actualEndDate = endDate ?? GetPeriodEndDate(budget);

            // Get transactions for the budget's category (or all if no category specified)
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

        public async Task<List<Budget>> GetBudgetsWithProgressAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var budgets = await _context.Budgets
                .Include(b => b.Category)
                .ToListAsync();

            // Calculate spent amount for each budget within the date range
            foreach (var budget in budgets)
            {
                budget.CurrentSpending = await CalculateSpentAmountAsync(budget.Id, startDate, endDate);
                budget.SpentAmount = budget.CurrentSpending; // Sync with NotMapped property if needed
            }

            return budgets;
        }

        private void SetDefaultDates(Budget budget)
        {
            var now = DateTime.Now;

            switch (budget.Period)
            {
                case BudgetPeriod.Daily:
                    budget.StartDate = now.Date;
                    budget.EndDate = now.Date.AddDays(1).AddSeconds(-1);
                    break;

                case BudgetPeriod.Weekly:
                    // Start from Monday of current week
                    var daysToMonday = ((int)now.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
                    budget.StartDate = now.Date.AddDays(-daysToMonday);
                    budget.EndDate = budget.StartDate.AddDays(7).AddSeconds(-1);
                    break;

                case BudgetPeriod.Monthly:
                    budget.StartDate = new DateTime(now.Year, now.Month, 1);
                    budget.EndDate = budget.StartDate.AddMonths(1).AddSeconds(-1);
                    break;

                case BudgetPeriod.Quarterly:
                    var quarter = (now.Month - 1) / 3;
                    budget.StartDate = new DateTime(now.Year, quarter * 3 + 1, 1);
                    budget.EndDate = budget.StartDate.AddMonths(3).AddSeconds(-1);
                    break;

                case BudgetPeriod.Yearly:
                    budget.StartDate = new DateTime(now.Year, 1, 1);
                    budget.EndDate = new DateTime(now.Year, 12, 31, 23, 59, 59);
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

        private DateTime GetPeriodStartDate(Budget budget)
        {
            return budget.StartDate;
        }

        private DateTime GetPeriodEndDate(Budget budget)
        {
            return budget.EndDate ?? DateTime.Now;
        }
    }
}
