using ExpenseTracker.Data.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace ExpenseTracker.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ICategoryService categoryService, ILogger<EditModel> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [BindProperty]
        public CategoryInputModel Category { get; set; } = new();
        public List<SelectListItem> TypeOptions { get; } = new()
        {
            new SelectListItem {Value = "0", Text = "Expense"},
            new SelectListItem {Value = "1", Text = "Income"},
        };

        public List<SelectListItem> IconOptions { get; } = CreateModel.IconOptions;
        public List<SelectListItem> ColorOptions { get; } = CreateModel.ColorOptions;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var category = await _categoryService.GetCategoryAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            Category = new CategoryInputModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Type = category.Type,
                Color = category.Color,
                Icon = category.Icon,
                Keywords = category.Keywords,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var category = await _categoryService.GetCategoryAsync(Category.Id);
                if(category == null)
                {
                    return NotFound();
                }

                // Update category
                category.Name = Category.Name;
                category.Description = Category.Description;
                category.Type = Category.Type;
                category.Color = Category.Color;
                category.Icon = Category.Icon;
                category.KeywordsJson = System.Text.Json.JsonSerializer.Serialize(Category.Keywords);
                category.ModifiedDate = DateTime.Now;

                await _categoryService.UpdateCategoryAsync(category);

                _logger.LogInformation("Category updated: {CategoryName} (ID: {CategoryId})", category.Name, category.Id);
                TempData["SuccessMessage"] = $"Category '{category.Name}' updated successfully!";

                return RedirectToPage("Index");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error updating category ID: {CategoryId}", Category.Id);
                ModelState.AddModelError("", "An error occurred while updating the category. Please try again.");
                return Page();
            }
        }

        public class CategoryInputModel : CreateModel.CategoryInputModel
        {
            public int Id { get; set; }
        }
    }
}
