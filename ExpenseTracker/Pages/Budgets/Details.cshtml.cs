using ExpenseTracker.Data.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Pages.Budgets
{
    public class DetailsModel : PageModel
    {
        private readonly IBudgetService _budgetService;
        private readonly ITransactionService _transactionService;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(
            IBudgetService budgetService,
            ITransactionService transactionService,
            ILogger<DetailsModel> logger)
        {
            _budgetService = budgetService;
            _transactionService = transactionService;
            _logger = logger;
        }

        public Budget Budget { get; set; }
        public List<Transaction> BudgetTransactions { get; set; } = new();
        public decimal DailyAverage { get; set; }
        public decimal WeeklyAverage { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public int DaysRemaining { get; set; }
        public int DaysElapsed { get; set; }
        public bool IsPeriodActive { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                // Get budget with spending calculated
                Budget = await _budgetService.GetBudgetWithSpendingAsync(id);

                if (Budget == null)
                {
                    ErrorMessage = "Budget not found.";
                    return RedirectToPage("Index");
                }

                // Get transactions for this budget
                await LoadBudgetTransactionsAsync(id);

                // Calculate averages and period info
                CalculateAverages();
                CalculatePeriodInfo();

                _logger.LogInformation("Budget details viewed: {BudgetName} (ID: {BudgetId})", Budget.Name, Budget.Id);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading budget details. BudgetId: {BudgetId}", id);
                ErrorMessage = "Error loading budget details. Please try again.";
                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostResetAsync(int id)
        {
            try
            {
                await _budgetService.ResetExpiredBudgets(); // This resets all expired budgets

                // Get the updated budget
                Budget = await _budgetService.GetBudgetWithSpendingAsync(id);

                SuccessMessage = $"Budget '{Budget.Name}' has been reset for the new period.";
                return RedirectToPage("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting budget. BudgetId: {BudgetId}", id);
                ErrorMessage = "Error resetting budget. Please try again.";
                return RedirectToPage("Details", new { id });
            }
        }

        public async Task<IActionResult> OnPostEnableNotificationsAsync(int id, bool enableExceeded = true, bool enableWarning = true)
        {
            try
            {
                var budget = await _budgetService.GetBudgetAsync(id);
                if (budget == null)
                {
                    ErrorMessage = "Budget not found.";
                    return RedirectToPage("Index");
                }

                budget.NotifyOnExceed = enableExceeded;
                budget.NotifyOnWarning = enableWarning;
                budget.ModifiedDate = DateTime.Now;

                await _budgetService.UpdateBudgetAsync(budget);

                SuccessMessage = $"Notification settings updated for '{budget.Name}'.";
                return RedirectToPage("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification settings. BudgetId: {BudgetId}", id);
                ErrorMessage = "Error updating notification settings.";
                return RedirectToPage("Details", new { id });
            }
        }

        private async Task LoadBudgetTransactionsAsync(int budgetId)
        {
            // Determine date range for transactions
            var startDate = Budget.StartDate;
            var endDate = Budget.EndDate ?? DateTime.Now;

            // Get all transactions in the date range
            var transactions = await _transactionService.GetTransactionsAsync(startDate, endDate);

            // Filter transactions based on budget category
            if (Budget.CategoryId.HasValue)
            {
                BudgetTransactions = transactions
                    .Where(t => t.CategoryId == Budget.CategoryId.Value && t.Type == TransactionType.Expense)
                    .OrderByDescending(t => t.Date)
                    .ToList();
            }
            else
            {
                // If no category, include all expense transactions
                BudgetTransactions = transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .OrderByDescending(t => t.Date)
                    .ToList();
            }
        }

        private void CalculateAverages()
        {
            if (BudgetTransactions.Any())
            {
                var totalSpent = BudgetTransactions.Sum(t => t.Amount);
                var daysInPeriod = (DateTime.Now - Budget.StartDate).TotalDays;

                if (daysInPeriod > 0)
                {
                    DailyAverage = totalSpent / (decimal)daysInPeriod;
                    WeeklyAverage = DailyAverage * 7;
                }
            }
        }

        private void CalculatePeriodInfo()
        {
            PeriodEndDate = Budget.EndDate ?? Budget.NextResetDate;

            if (PeriodEndDate.HasValue)
            {
                var now = DateTime.Now;
                DaysRemaining = (PeriodEndDate.Value - now).Days;
                DaysElapsed = (now - Budget.StartDate).Days;
                IsPeriodActive = now >= Budget.StartDate && now <= PeriodEndDate.Value;
            }
            else
            {
                // Ongoing budget with no end date
                DaysRemaining = -1;
                DaysElapsed = (DateTime.Now - Budget.StartDate).Days;
                IsPeriodActive = true;
            }
        }
    }
}