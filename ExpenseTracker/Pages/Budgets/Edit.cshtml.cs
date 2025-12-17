using ExpenseTracker.Data.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public decimal CurrentSpending { get; private set; }
        public decimal RemainingAmount { get; private set; }
        public decimal PercentageUsed { get; private set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var existingBudget = await _budgetService.GetBudgetWithSpendingAsync(id);

                if (existingBudget == null)
                {
                    ErrorMessage = "Budget not found.";
                    return RedirectToPage("Index");
                }

                await LoadCategoriesAsync();

                // Populate the input model with existing budget data
                Budget = new BudgetInputModel
                {
                    Id = existingBudget.Id,
                    Name = existingBudget.Name,
                    Description = existingBudget.Description,
                    Amount = existingBudget.Amount,
                    Period = existingBudget.Period,
                    StartDate = existingBudget.StartDate,
                    EndDate = existingBudget.EndDate,
                    CategoryId = existingBudget.CategoryId ?? 0,
                    NotifyOnExceed = existingBudget.NotifyOnExceed,
                    NotifyOnWarning = existingBudget.NotifyOnWarning,
                    WarningThreshold = existingBudget.WarningThreshold
                };

                CurrentSpending = existingBudget.CurrentSpending;
                RemainingAmount = existingBudget.RemainingAmount;
                PercentageUsed = existingBudget.PercentageUsed;

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading budget for editing. BudgetId: {BudgetId}", id);
                ErrorMessage = "Error loading budget. Please try again.";
                return RedirectToPage("Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadCategoriesAsync();

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Budget update failed validation for budget ID: {BudgetId}", Budget.Id);
                return Page();
            }

            try
            {
                // Get current budget to preserve calculated fields
                var existingBudget = await _budgetService.GetBudgetAsync(Budget.Id);
                if (existingBudget == null)
                {
                    ErrorMessage = "Budget not found.";
                    return RedirectToPage("Index");
                }

                // Update properties
                existingBudget.Name = Budget.Name;
                existingBudget.Description = Budget.Description;
                existingBudget.Amount = Budget.Amount;
                existingBudget.Period = Budget.Period;
                existingBudget.StartDate = Budget.StartDate;
                existingBudget.EndDate = Budget.EndDate;
                existingBudget.CategoryId = Budget.CategoryId == 0 ? null : Budget.CategoryId;
                existingBudget.NotifyOnExceed = Budget.NotifyOnExceed;
                existingBudget.NotifyOnWarning = Budget.NotifyOnWarning;
                existingBudget.WarningThreshold = Budget.WarningThreshold;

                await _budgetService.UpdateBudgetAsync(existingBudget);

                _logger.LogInformation("Budget updated: {BudgetName} (ID: {BudgetId})", existingBudget.Name, existingBudget.Id);
                SuccessMessage = $"Budget '{existingBudget.Name}' updated successfully!";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating budget ID: {BudgetId}", Budget.Id);
                ErrorMessage = "An error occurred while updating the budget. Please try again.";
                ModelState.AddModelError("", ErrorMessage);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostResetAsync(int id)
        {
            try
            {
                var budget = await _budgetService.GetBudgetAsync(id);
                if (budget == null)
                {
                    ErrorMessage = "Budget not found.";
                    return RedirectToPage("Index");
                }

                // Reset logic would go here - you might want to add a ResetBudgetAsync method
                // For now, we'll just update the dates
                budget.LastResetDate = DateTime.Now;
                budget.NextResetDate = CalculateNextResetDate(budget, DateTime.Now);

                await _budgetService.UpdateBudgetAsync(budget);

                SuccessMessage = $"Budget '{budget.Name}' has been reset for the new period.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting budget ID: {BudgetId}", id);
                ErrorMessage = "Error resetting budget. Please try again.";
                return RedirectToPage("Index");
            }
        }

        private DateTime? CalculateNextResetDate(Budget budget, DateTime currentDate)
        {
            return budget.Period switch
            {
                BudgetPeriod.Daily => currentDate.AddDays(1),
                BudgetPeriod.Weekly => currentDate.AddDays(7),
                BudgetPeriod.Monthly => currentDate.AddMonths(1),
                BudgetPeriod.Quarterly => currentDate.AddMonths(3),
                BudgetPeriod.Yearly => currentDate.AddYears(1),
                BudgetPeriod.Custom => null,
                _ => null
            };
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
                Text = c.Name,
                Selected = Budget.CategoryId == c.Id
            }));
        }

        public class BudgetInputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Budget name is required")]
            [StringLength(100, ErrorMessage = "Budget name cannot exceed 100 characters")]
            [Display(Name = "Budget Name")]
            public string Name { get; set; } = string.Empty;

            [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
            [Display(Name = "Description (Optional)")]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Budget amount is required")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
            [DataType(DataType.Currency)]
            [Display(Name = "Budget Amount")]
            public decimal Amount { get; set; }

            [Required(ErrorMessage = "Please select a period")]
            [Display(Name = "Budget Period")]
            public BudgetPeriod Period { get; set; }

            [Required(ErrorMessage = "Start date is required")]
            [DataType(DataType.Date)]
            [Display(Name = "Start Date")]
            public DateTime StartDate { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "End Date (Optional)")]
            public DateTime? EndDate { get; set; }

            [Display(Name = "Category (Optional)")]
            public int CategoryId { get; set; }

            [Display(Name = "Notify when budget is exceeded")]
            public bool NotifyOnExceed { get; set; } = true;

            [Display(Name = "Notify when approaching budget limit")]
            public bool NotifyOnWarning { get; set; } = true;

            [Range(50, 95, ErrorMessage = "Warning threshold must be between 50% and 95%")]
            [Display(Name = "Warning Threshold (%)")]
            public int WarningThreshold { get; set; } = 80;
        }
    }
}