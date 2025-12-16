using ExpenseTracker.Data.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ExpenseTracker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ITransactionService _transactionService;
        private readonly IBudgetService _budgetService;

        public DashboardStats DashboardStats { get; set; }
        public List<Budget> Budgets { get; set; }
        public List<Transaction> RecentTransactions { get; set; }


        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; } = DateTime.Now.AddMonths(-1);

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; } = DateTime.Now;

        public IndexModel(ITransactionService transactionService, IBudgetService budgetService)
        {
            _transactionService = transactionService;
            _budgetService = budgetService;
        }

        public async Task OnGetAsync()
        {
            //Generating any recurring tranactions first
            await _transactionService.GetRecurringTransactionsAsync();
            DashboardStats = await _transactionService.GetDashboardStatsAsync(StartDate, EndDate);
            Budgets = await _budgetService.GetBudgetsWithProgressAsync(StartDate, EndDate);
            RecentTransactions = await _transactionService.GetTransactionsAsync(DateTime.Now.AddDays(-7), DateTime.Now, null, null);

            //Take last 5 transactions for dashboard
            RecentTransactions = RecentTransactions.Take(5).ToList();

        }
    }
}
