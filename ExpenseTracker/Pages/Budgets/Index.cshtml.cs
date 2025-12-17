using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Pages.Budgets
{
    public class IndexModel : PageModel
    {
        private readonly IBudgetService _budgetService;

        public List<Data.Models.Budget> Budgets { get; set; } = new();
        public decimal TotalBudgeted { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal TotalRemaining => TotalBudgeted - TotalSpent;
        public decimal OverallPercentage => TotalBudgeted > 0 ? (TotalSpent / TotalBudgeted) * 100 : 0;

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public IndexModel(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        public async Task OnGetAsync()
        {
            try
            {
                Budgets = await _budgetService.GetBudgetsAsync();
                TotalBudgeted = Budgets.Sum(b => b.Amount);
                TotalSpent = Budgets.Sum(b => b.CurrentSpending);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error loading budgets: {ex.Message}";
                Budgets = new List<Data.Models.Budget>();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _budgetService.DeleteBudgetAsync(id);
                SuccessMessage = "Budget deleted successfully.";
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error deleting budget: {ex.Message}";
            }
            return RedirectToPage();
        }
    }
}