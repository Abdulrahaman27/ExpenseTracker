using ExpenseTracker.Data.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;

namespace ExpenseTracker.Pages.Categories
{
    public class DetailsModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly ITransactionService _transactionService;

        public DetailsModel(ICategoryService categoryService, ITransactionService transactionService)
        {
            _categoryService = categoryService;
            _transactionService = transactionService;
        }

        public Category Category { get; set; } = new();
        public int TransactionCount { get; set; }
        public decimal TotalAmount { get; set; }
        public List<Transaction> RecentTransactions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var category = await _categoryService.GetCategoryAsync(id);
            if(category == null)
            {
                return NotFound();  
            }

            Category = category;

            //Get transaction statistics
            var transactions = await _transactionService.GetTransactionsByCategoryAsync(id);
            TransactionCount = transactions.Count;
            TotalAmount = transactions.Sum(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);
            RecentTransactions = transactions
                .OrderByDescending(t => t.Date)
                .Take(10)
                .ToList();

            return Page();
        }
    }
}
