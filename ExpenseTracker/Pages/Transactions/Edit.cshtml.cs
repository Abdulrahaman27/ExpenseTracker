using ExpenseTracker.Data.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Pages.Transactions
{
    public class EditModel : PageModel
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;

        [BindProperty]
        public Transaction Transaction { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();

        // For recurring transaction history
        public List<Transaction> RecurringHistory { get; set; } = new List<Transaction>();

        public EditModel(ITransactionService transactionService, ICategoryService categoryService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Transaction = await _transactionService.GetTransactionAsync(id);

            if (Transaction == null)
            {
                return NotFound();
            }

            Categories = await _categoryService.GetCategoriesAsync();

            // Load recurring transaction history if applicable
            if (Transaction.IsRecurring)
            {
                var allTransactions = await _transactionService.GetTransactionsAsync();
                RecurringHistory = allTransactions
                    .Where(t => t.Description == Transaction.Description &&
                               t.CategoryId == Transaction.CategoryId &&
                               t.Id != Transaction.Id)
                    .OrderByDescending(t => t.Date)
                    .ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Categories = await _categoryService.GetCategoriesAsync();
                return Page();
            }

            await _transactionService.UpdateTransactionAsync(Transaction);

            TempData["SuccessMessage"] = $"Transaction '{Transaction.Description}' updated successfully!";

            return RedirectToPage("Edit", new { id = Transaction.Id });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _transactionService.DeleteTransactionAsync(id);

            TempData["SuccessMessage"] = "Transaction deleted successfully!";

            return RedirectToPage("Index");
        }
    }
}