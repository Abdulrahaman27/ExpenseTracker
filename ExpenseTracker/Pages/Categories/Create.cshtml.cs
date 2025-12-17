using ExpenseTracker.Data.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ExpenseTracker.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CreateModel> _logger;
        public CreateModel(ICategoryService categoryService, ILogger<CreateModel> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [BindProperty]
        public CategoryInputModel Category { get; set; } = new();
        public List<SelectListItem> TypeOptions { get; } = new()
        {
            new SelectListItem { Value = "0", Text = "Expense" },
            new SelectListItem { Value = "1", Text = "Income" }
        };

        //common icons for categories
        public static List<SelectListItem> IconOptions { get; } = new()
        {
            new SelectListItem { Value = "bi-tag", Text = "Tag" },
            new SelectListItem { Value = "bi-cart", Text = "Cart" },
            new SelectListItem { Value = "bi-car-front", Text = "Car" },
            new SelectListItem { Value = "bi-house", Text = "House" },
            new SelectListItem { Value = "bi-cup-straw", Text = "Food/Drink" },
            new SelectListItem { Value = "bi-film", Text = "Entertainment" },
            new SelectListItem { Value = "bi-heart", Text = "Health" },
            new SelectListItem { Value = "bi-cash-coin", Text = "Money" },
            new SelectListItem { Value = "bi-laptop", Text = "Tech" },
            new SelectListItem { Value = "bi-gift", Text = "Gift" },
            new SelectListItem { Value = "bi-airplane", Text = "Travel" },
            new SelectListItem { Value = "bi-phone", Text = "Phone" },
            new SelectListItem { Value = "bi-wifi", Text = "Internet" },
            new SelectListItem { Value = "bi-droplet", Text = "Utilities" },
            new SelectListItem { Value = "bi-book", Text = "Education" },
            new SelectListItem { Value = "bi-shop", Text = "Shopping" }
        };

        //common color for categories
        public static List<SelectListItem> ColorOptions { get; } = new()
        {
            new SelectListItem { Value = "#dc3545", Text = "Red" },
            new SelectListItem { Value = "#fd7e14", Text = "Orange" },
            new SelectListItem { Value = "#ffc107", Text = "Yellow" },
            new SelectListItem { Value = "#198754", Text = "Green" },
            new SelectListItem { Value = "#0dcaf0", Text = "Teal" },
            new SelectListItem { Value = "#0d6efd", Text = "Blue" },
            new SelectListItem { Value = "#6f42c1", Text = "Purple" },
            new SelectListItem { Value = "#d63384", Text = "Pink" },
            new SelectListItem { Value = "#6c757d", Text = "Gray" },
            new SelectListItem { Value = "#20c997", Text = "Cyan" },
            new SelectListItem { Value = "#6610f2", Text = "Indigo" }
        };

        public void OnGet()
        {
            // Set default values
            Category.Type = TransactionType.Expense;
            Category.Color = "#6c757d";
            Category.Icon = "bi-tag";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                _logger.LogWarning("Category creation failed validation");
                return Page();
            }

            try
            {
                var category = new Category
                {
                    Name = Category.Name,
                    Description = Category.Description,
                    Type = Category.Type,
                    Color = Category.Color,
                    Icon = Category.Icon,
                    KeywordsJson = System.Text.Json.JsonSerializer.Serialize(Category.Keywords)
                };
                await _categoryService.CreateCategoryAsync(category);

                _logger.LogInformation("Category created: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);
                TempData["SuccessMessage"] = $"Category '{category.Name}' created successfully!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                ModelState.AddModelError("", "An error occurred while creating the category. Please try again."); 
                return Page();
            }
        }

        public class CategoryInputModel
        {
            [Required(ErrorMessage = "Category name is required")]
            [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
            [Display(Name = "Category Name")]
            public string Name { get; set; } = string.Empty;

            [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
            [Display(Name = "Description (Optional)")]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Please select a category type")]
            [Display(Name = "Category Type")]
            public TransactionType Type { get; set; }

            [Required(ErrorMessage = "Please select a color")]
            [Display(Name = "Color")]
            public string Color { get; set; } = "#6c757d";

            [Required(ErrorMessage = "Please select an icon")]
            [Display(Name = "Icon")]
            public string Icon { get; set; } = "bi-tag";

            [Display(Name = "Keywords (for smart suggestions)")]
            public List<string> Keywords { get; set; } = new();

        }
    }
}
