using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
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

        public IndexModel(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }


        public async Task OnGetAsync()
        {
            Budgets = await _budgetService.GetBudgetsAsync();

            TotalBudgeted = Budgets.Sum(b => b.Amount);
            TotalSpent = Budgets.Sum(b => b.CurrentSpending);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _budgetService.DeleteBudgetAsync(id);
            TempData["SuccessMessage"] = "Budget deleted successfully.";
            return RedirectToPage();
        }
    }
}
