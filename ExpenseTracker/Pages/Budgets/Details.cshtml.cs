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
        private readonly ICategoryService _categoryService;

        public DetailsModel(
            IBudgetService budgetService,
            ITransactionService transactionService,
            ICategoryService categoryService)
        {
            _budgetService = budgetService;
            _transactionService = transactionService;
            _categoryService = categoryService;
        }
        public Budget Budget { get; set; } = new();
        public List<Transaction> RecentTransactions { get; set; } = new();
        public decimal TotalSpent { get; set; }
        public decimal RemainingAmount => Budget.Amount - TotalSpent;
        public decimal PercentageUsed => Budget.Amount > 0 ? (TotalSpent / Budget.Amount) * 100 : 0;
        public bool IsOverBudget => TotalSpent > Budget.Amount;

        public string ProgressBarColor
        {
            get
            {
                return PercentageUsed switch
                {
                    var p when p >= 100 => "bg-danger",
                    var p when p >= 80 => "bg-warning",
                    _ => "bg-success"
                };
            }
        }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            var budget = await _budgetService.GetBudgetWithSpendingAsync(id);
            if (budget == null)
            {
                return NotFound();
            }

            Budget = budget;
            TotalSpent = budget.CurrentSpending;

            // Get recent transactions for this budget
            await LoadRecentTransactionsAsync();

            return Page();
        }

        private async Task LoadRecentTransactionsAsync()
        {
            // Determine date range based on budget period
            DateTime startDate = Budget.StartDate;
            DateTime endDate = Budget.EndDate ?? DateTime.Now;

            // Get transactions for the date range
            var transactions = await _transactionService.GetTransactionsAsync(startDate, endDate);

            // Filter by category if budget is category-specific
            if (Budget.CategoryId.HasValue)
            {
                RecentTransactions = transactions
                    .Where(t => t.CategoryId == Budget.CategoryId.Value && t.Type == TransactionType.Expense)
                    .OrderByDescending(t => t.Date)
                    .Take(10)
                    .ToList();
            }
            else
            {
                // If no category specified, get all expense transactions
                RecentTransactions = transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .OrderByDescending(t => t.Date)
                    .Take(10)
                    .ToList();
            }
        }
    }
}
