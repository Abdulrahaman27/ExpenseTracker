using ExpenseTracker.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    public interface IReportService
    {
        Task<FinancialReport> GenerateMonthlyReportAsync(int year, int month);
        Task<FinancialReport> GenerateYearlyReportAsync(int year);
        Task<FinancialReport> GenerateCustomReportAsync(DateTime startDate, DateTime endDate);
        Task<List<MonthlySummary>> GetMonthlySummariesAsync(int year);
        Task<CategoryReport> GetCategoryReportAsync(DateTime startDate, DateTime endDate);
    }

    public class FinancialReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetBalance => TotalIncome - TotalExpenses;
        public List<CategorySummary> CategorySummaries { get; set; }
        public List<Transaction> TopTransactions { get; set; }
        public BudgetSummary BudgetSummary { get; set; }
    }

    public class CategorySummary
    {
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
        public int TransactionCount { get; set; }
    }

    public class MonthlySummary
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public decimal Balance => Income - Expenses;
    }

    public class CategoryReport
    {
        public List<CategorySpending> CategorySpendings { get; set; }
        public List<Category> TopCategories { get; set; }
        public decimal AverageTransactionAmount { get; set; }
    }

    public class BudgetSummary
    {
        public int TotalBudgets { get; set; }
        public int OverBudgetCount { get; set; }
        public int UnderBudgetCount { get; set; }
        public List<BudgetStatus> BudgetStatuses { get; set; }
    }

    public class BudgetStatus
    {
        public string BudgetName { get; set; }
        public string CategoryName { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal ActualSpending { get; set; }
        public decimal Difference => BudgetAmount - ActualSpending;
        public bool IsOverBudget => ActualSpending > BudgetAmount;
    }
}