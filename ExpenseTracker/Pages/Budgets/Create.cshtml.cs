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
    public class CreateModel : PageModel
    {
        private readonly IBudgetService _budgetService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(
            IBudgetService budgetService,
            ICategoryService categoryService,
            ILogger<CreateModel> logger)
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

        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();

            // Set default values
            Budget.Period = BudgetPeriod.Monthly;
            Budget.StartDate = DateTime.Now;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadCategoriesAsync();

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Budget creation failed validation");
                return Page();
            }

            try
            {
                var budget = new Budget
                {
                    Name = Budget.Name,
                    Description = Budget.Description,
                    Amount = Budget.Amount,
                    Period = Budget.Period,
                    StartDate = Budget.StartDate,
                    EndDate = Budget.EndDate,
                    CategoryId = Budget.CategoryId == 0 ? null : Budget.CategoryId
                };

                await _budgetService.CreateBudgetAsync(budget);

                _logger.LogInformation("Budget created: {BudgetName} (ID: {BudgetId})", budget.Name, budget.Id);
                TempData["SuccessMessage"] = $"Budget '{budget.Name}' created successfully!";

                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating budget");
                ModelState.AddModelError("", "An error occurred while creating the budget. Please try again.");
                return Page();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            var allCategories = await _categoryService.GetCategoriesAsync();

            Categories = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "All Categories (Overall Budget)", Selected = true }
            };

            Categories.AddRange(allCategories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }));
        }

        public class BudgetInputModel
        {
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
        }
    }
}
