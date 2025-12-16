using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace ExpenseTracker.Pages.Transactions
{
    public class DetailsModel : PageModel
    {
        private readonly ITransactionService _transactionService;
        public Data.Models.Transaction Transaction { get; set; }
        public DetailsModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Transaction = await _transactionService.GetTransactionAsync(id);

            if (Transaction == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
