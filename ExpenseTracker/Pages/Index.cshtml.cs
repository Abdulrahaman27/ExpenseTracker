using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ITransactionService _transactionService;

        public DashboardStats? DashboardStats { get; set; }
        public List<Data.Models.Budget>? Budgets { get; set; }
        public List<Data.Models.Transaction>? RecentTransactions { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; } = DateTime.Now.AddMonths(-1);

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; } = DateTime.Now;

        public IndexModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task OnGetAsync()
        {
            try
            {
                DashboardStats = await _transactionService.GetDashboardStatsAsync(StartDate, EndDate);
                RecentTransactions = await _transactionService.GetTransactionsAsync(
                    DateTime.Now.AddDays(-30),
                    DateTime.Now,
                    null,
                    null);

                // Take only last 10 transactions
                RecentTransactions = RecentTransactions?.Take(10).ToList() ?? new List<Data.Models.Transaction>();
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                Console.WriteLine($"Error loading dashboard: {ex.Message}");
                DashboardStats = new DashboardStats();
                RecentTransactions = new List<Data.Models.Transaction>();
            }
        }
    }
}