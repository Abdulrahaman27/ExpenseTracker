using ExpenseTracker.Data;
using ExpenseTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionService _transactionService;
        private readonly IBudgetService _budgetService;

        public ReportService(
            ApplicationDbContext context,
            ITransactionService transactionService,
            IBudgetService budgetService)
        {
            _context = context;
            _transactionService = transactionService;
            _budgetService = budgetService;
        }

        public async Task<FinancialReport> GenerateMonthlyReportAsync(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return await GenerateCustomReportAsync(startDate, endDate);
        }

        public async Task<FinancialReport> GenerateYearlyReportAsync(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);

            return await GenerateCustomReportAsync(startDate, endDate);
        }

        public async Task<FinancialReport> GenerateCustomReportAsync(DateTime startDate, DateTime endDate)
        {
            var transactions = await _transactionService.GetTransactionsAsync(startDate, endDate);
            var budgets = await _budgetService.GetBudgetsWithProgressAsync(startDate, endDate);

            var report = new FinancialReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                TotalExpenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                TopTransactions = transactions
                    .OrderByDescending(t => t.Amount)
                    .Take(10)
                    .ToList(),
                BudgetSummary = new BudgetSummary
                {
                    TotalBudgets = budgets.Count,
                    OverBudgetCount = budgets.Count(b => b.CurrentSpending > b.Amount),
                    UnderBudgetCount = budgets.Count(b => b.CurrentSpending <= b.Amount),
                    BudgetStatuses = budgets.Select(b => new BudgetStatus
                    {
                        BudgetName = b.Name,
                        CategoryName = b.Category?.Name,
                        BudgetAmount = b.Amount,
                        ActualSpending = b.CurrentSpending
                    }).ToList()
                }
            };

            // Calculate category summaries
            var expenseTransactions = transactions.Where(t => t.Type == TransactionType.Expense).ToList();
            var totalExpenses = report.TotalExpenses;

            if (totalExpenses > 0)
            {
                report.CategorySummaries = expenseTransactions
                    .GroupBy(t => t.Category)
                    .Select(g => new CategorySummary
                    {
                        CategoryName = g.Key?.Name ?? "Uncategorized",
                        CategoryColor = g.Key?.Color ?? "#6c757d",
                        Amount = g.Sum(t => t.Amount),
                        Percentage = (g.Sum(t => t.Amount) / totalExpenses) * 100,
                        TransactionCount = g.Count()
                    })
                    .OrderByDescending(c => c.Amount)
                    .ToList();
            }
            else
            {
                report.CategorySummaries = new List<CategorySummary>();
            }

            return report;
        }

        public async Task<List<MonthlySummary>> GetMonthlySummariesAsync(int year)
        {
            var summaries = new List<MonthlySummary>();

            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var transactions = await _transactionService.GetTransactionsAsync(startDate, endDate);

                summaries.Add(new MonthlySummary
                {
                    Year = year,
                    Month = month,
                    MonthName = startDate.ToString("MMMM"),
                    Income = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                });
            }

            return summaries;
        }

        public async Task<CategoryReport> GetCategoryReportAsync(DateTime startDate, DateTime endDate)
        {
            var transactions = await _transactionService.GetTransactionsAsync(startDate, endDate);
            var expenseTransactions = transactions.Where(t => t.Type == TransactionType.Expense).ToList();

            var categorySpendings = expenseTransactions
                .GroupBy(t => t.Category)
                .Select(g => new CategorySpending
                {
                    CategoryName = g.Key?.Name ?? "Uncategorized",
                    CategoryColor = g.Key?.Color ?? "#6c757d",
                    Amount = g.Sum(t => t.Amount),
                    Percentage = 0 // Will calculate below
                })
                .OrderByDescending(c => c.Amount)
                .ToList();

            var totalExpenses = categorySpendings.Sum(c => c.Amount);
            if (totalExpenses > 0)
            {
                foreach (var category in categorySpendings)
                {
                    category.Percentage = (category.Amount / totalExpenses) * 100;
                }
            }

            return new CategoryReport
            {
                CategorySpendings = categorySpendings,
                TopCategories = categorySpendings
                    .Take(5)
                    .Select(c => new Category
                    {
                        Name = c.CategoryName,
                        Color = c.CategoryColor
                    })
                    .ToList(),
                AverageTransactionAmount = expenseTransactions.Any()
                    ? expenseTransactions.Average(t => t.Amount)
                    : 0
            };
        }
    }
}