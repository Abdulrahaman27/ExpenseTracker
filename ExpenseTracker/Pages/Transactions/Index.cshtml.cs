using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Pages.Transactions
{
    public class IndexModel : PageModel
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;

        public List<Data.Models.Transaction> Transactions { get; set; } = new List<Data.Models.Transaction>();
        public List<Data.Models.Category> Categories { get; set; } = new List<Data.Models.Category>();

        //Filter properties
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; } = DateTime.Now.AddMonths(-1);

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; } = DateTime.Now;

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Search {  get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Type { get; set; }

        //Pagination
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }


        //Summary Statistics
        public decimal TotalIncome  { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetBalance => TotalIncome - TotalExpenses;

        public IndexModel(ITransactionService transactionService, ICategoryService categoryService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
        }

        public async Task OnGetAsync()
        {
            //Load categories from dropdown
            Categories = await _categoryService.GetCategoriesAsync();

            //Get filtered Transactions
            var allTransactions = await _transactionService.GetTransactionsAsync(
                StartDate, EndDate, CategoryId, Search);

            //filter by type if specified
            if (!string.IsNullOrEmpty(Type) && Enum.TryParse<Data.Models.TransactionType>(Type, out var transactionType))
            {
                allTransactions = allTransactions.Where(t => t.Type == transactionType).ToList(); ;
            }

            //Calculate summary statistics
            TotalIncome = allTransactions.Where(t => t.Type == Data.Models.TransactionType.Income).Sum(t => t.Amount);
            TotalExpenses = allTransactions.Where(t => t.Type == Data.Models.TransactionType.Expense).Sum(t => t.Amount);

            //Apply pagination
            TotalCount = allTransactions.Count;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            Transactions = allTransactions
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _transactionService.DeleteTransactionAsync(id);
            return RedirectToPage(new
            {
                StartDate,
                EndDate,
                CategoryId,
                Search,
                Type,
                CurrentPage
            });
        }

        public IActionResult OnPostClearFilters()
        {
            return RedirectToPage(new { CurrentPage = 1 });
        }
    }
}
