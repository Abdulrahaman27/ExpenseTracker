using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTracker.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ICategoryService _categoryService;

        public List<Data.Models.Category> Categories { get; set; } = new();
        public Dictionary<int, int> TransactionCounts { get; set; } = new();

        public IndexModel(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task OnGet()
        {
            Categories = await _categoryService.GetCategoriesAsync();

            //Get transaction counts for each category
            foreach (var category in Categories)
            {
                var count = await _categoryService.GetTransactionCountAsync(category.Id);
                TransactionCounts[category.Id] = count;
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the category.";
            }

            return RedirectToPage();
        }
    }
}
