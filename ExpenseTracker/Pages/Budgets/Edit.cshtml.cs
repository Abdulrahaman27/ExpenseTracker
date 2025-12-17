using ExpenseTracker.Data.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ExpenseTracker.Pages.Budgets.CreateModel;

namespace ExpenseTracker.Pages.Budgets
{
    public class EditModel : PageModel
    {
        private readonly IBudgetService _budgetService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(
            IBudgetService budgetService,
            ICategoryService categoryService,
            ILogger<EditModel> logger)
        {
            _budgetService = budgetService;
            _categoryService = categoryService;
            _logger = logger;
        }

        [BindProperty]
        public BudgetInputModel Budget { get; set; } = new();

        public List<SelectListItem> PeriodOptions { get; } = new()
        {
            new SelectListItem { Value = "0", Text = "Daily" },
            new SelectListItem { Value = "1", Text = "Weekly" },
            new SelectListItem { Value = "2", Text = "Monthly" },
            new SelectListItem { Value = "3", Text = "Quarterly" },
            new SelectListItem { Value = "4", Text = "Yearly" },
            new SelectListItem { Value = "5", Text = "Custom" }
        };

        public List<SelectListItem> Categories { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            await LoadCategoriesAsync();

            var budget = await _budgetService.GetBudgetAsync(id);
            if (budget == null)
            {
                return NotFound();
            }

            Budget = new BudgetInputModel
            {
                Id = budget.Id,
                Name = budget.Name,
                Description = budget.Description,
                Amount = budget.Amount,
                Period = budget.Period,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                CategoryId = budget.CategoryId ?? 0
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadCategoriesAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var budget = await _budgetService.GetBudgetAsync(Budget.Id);
                if (budget == null)
                {
                    return NotFound();
                }

                // Update budget
                budget.Name = Budget.Name;
                budget.Description = Budget.Description;
                budget.Amount = Budget.Amount;
                budget.Period = Budget.Period;
                budget.StartDate = Budget.StartDate;
                budget.EndDate = Budget.EndDate;
                budget.CategoryId = Budget.CategoryId == 0 ? null : Budget.CategoryId;
                budget.ModifiedDate = DateTime.Now;

                await _budgetService.UpdateBudgetAsync(budget);

                _logger.LogInformation("Budget updated: {BudgetName} (ID: {BudgetId})", budget.Name, budget.Id);
                TempData["SuccessMessage"] = $"Budget '{budget.Name}' updated successfully!";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating budget ID: {BudgetId}", Budget.Id);
                ModelState.AddModelError("", "An error occurred while updating the budget. Please try again.");
                return Page();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            var allCategories = await _categoryService.GetCategoriesAsync();

            Categories = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "All Categories (Overall Budget)" }
            };

            Categories.AddRange(allCategories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }));
        }

        public class BudgetInputModel : CreateModel.BudgetInputModel
        {
            public int Id { get; set; }
        }

    }
}
