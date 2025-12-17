using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }


        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }


        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Search {  get; set; }

        [BindProperty(SupportsGet = true)]
        public Data.Models.TransactionType? Type { get; set; }

        [BindProperty]
        public string? SelectedIds { get; set; }

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
            if (!StartDate.HasValue)
            {
                StartDate = DateTime.Now.AddMonths(-1).Date;
            }

            if (!EndDate.HasValue)
            {
                EndDate = DateTime.Now.Date;
            }
            //Load categories from dropdown
            Categories = await _categoryService.GetCategoriesAsync();

            //Get filtered Transactions
            var allTransactions = await _transactionService.GetTransactionsAsync(
                StartDate,
                EndDate?.Date.AddDays(1).AddSeconds(-1),
                CategoryId,
                Search);



            //filter by type if specified
            if (Type.HasValue)
            {
                allTransactions = allTransactions.Where(t => t.Type == Type.Value).ToList(); ;
            }

            //Calculate summary statistics
            TotalIncome = allTransactions.Where(t => t.Type == Data.Models.TransactionType.Income).Sum(t => t.Amount);
            TotalExpenses = allTransactions.Where(t => t.Type == Data.Models.TransactionType.Expense).Sum(t => t.Amount);

            //Apply pagination
            TotalCount = allTransactions.Count;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            // Handle invalid page number
            if(CurrentPage < 1) CurrentPage = 1;
            if(CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;

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
                StartDate = StartDate?.ToString("yyyy-MM-dd"),
                EndDate = EndDate?.ToString("yyyy-MM-dd"),
                CategoryId,
                Search,
                Type = Type?.ToString(),
                CurrentPage
            });
        }

        // Delete selected Ids
        public async Task<IActionResult> OnPostDeleteSelectedAsync()
        {
            if (!string.IsNullOrEmpty(SelectedIds))
            {
                var ids = SelectedIds.Split(',')
                .Select(id => int.TryParse(id, out var result) ? result : 0)
                .Where(id => id > 0)
                .ToList();

                if (ids.Any())
                {
                    foreach (var id in ids)
                    {
                        await _transactionService.DeleteTransactionAsync(id);    
                    }
                    TempData["SuccessMessage"] = $"Successfully deleted {ids.Count} transaction(s).";
                }
            }

            //Preserve filters
            return RedirectToPage(new
            {
                StartDate = StartDate?.ToString("yyyy-MM-dd"),
                EndDate = EndDate?.ToString("yyyy-MM-dd"),
                CategoryId,
                Search,
                Type = Type?.ToString(),
                CurrentPage
            });
        }

        public IActionResult OnPostClearFilters()
        {
            return RedirectToPage(new { CurrentPage = 1 });
        }
    }
}
