using ExpenseTracker.Data;
using ExpenseTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ExpenseTracker.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionsAsync(DateTime? startDate = null, DateTime? endDate = null,
            int? categoryId = null,
            string search = null);
        Task<Transaction> GetTransactionAsync(int id);
        Task CreateTransactionAsync(Transaction transaction);
        Task UpdateTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(int id);
        Task<DashboardStats> GetDashboardStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Category> SuggestCategoryAsync(string description);
        Task<List<Transaction>> GetRecurringTransactionsAsync();
        Task GenerateRecurringTransactionsAsync();
        Task<byte[]> ExportToCsvAsync(DateTime? startDate = null, DateTime? endDate = null);
    }

    public class DashboardStats
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance => TotalIncome - TotalExpenses;
        public List<CategorySpending> CategorySpendings { get; set; }
        public List<MonthlyTrend> MonthlyTrends { get; set; }
    }

    public class CategorySpending
    {
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class MonthlyTrend
    {
        public string Month {  get; set; }
        public decimal Income {  get; set; }
        public decimal Expenses {  get; set; }
    }

    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryService _categoryService;

        public TransactionService(ApplicationDbContext context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        public async Task<List<Transaction>> GetTransactionsAsync(DateTime? startDate = null, DateTime? endDate = null, 
            int? categoryId = null,
            string search = null)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .AsQueryable();

            if(startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);
            if(endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);
            if(categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if(!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Description.Contains(search) || t.Notes.Contains(search));
            return await query.OrderByDescending(t => t.Date).ToListAsync();
        }

        public async Task<DashboardStats> GetDashboardStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var transactions = await GetTransactionsAsync(startDate, endDate);

            var stats = new DashboardStats
            {
                TotalIncome = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                TotalExpenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount),
                CategorySpendings = transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .GroupBy(t => t.Category)
                    .Select(g => new CategorySpending
                    {
                        CategoryName = g.Key?.Name ?? "Uncategorized",
                        CategoryColor = g.Key?.Color ?? "#6c757d",
                        Amount = g.Sum(t => t.Amount),
                        Percentage = 0
                    })
                    .OrderByDescending(c => c.Amount)
                    .ToList()
            };

            // Calculate percentages
            var totalExpenses = stats.TotalExpenses;
            if (totalExpenses > 0)
            {
                foreach (var category in stats.CategorySpendings)
                {
                    category.Percentage = (category.Amount / totalExpenses) * 100;
                }
            }

            // Get monthly trend for last 6 months - FIXED VERSION
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            var monthlyData = await _context.Transactions
                .Where(t => t.Date >= sixMonthsAgo)
                .GroupBy(t => new {
                    Year = t.Date.Year,
                    Month = t.Date.Month
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Income = g.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                    Expenses = g.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
                })
                .OrderBy(m => m.Year).ThenBy(m => m.Month)
                .ToListAsync();

            // Format on client side
            stats.MonthlyTrends = monthlyData.Select(g => new MonthlyTrend
            {
                Month = $"{g.Year}-{g.Month:D2}",
                Income = g.Income,
                Expenses = g.Expenses
            }).ToList();

            return stats;
        }
        public async Task<Category> SuggestCategoryAsync(string description)
        {
            if (string.IsNullOrEmpty(description))
                return null;

            var categories = await _categoryService.GetCategoriesAsync();
            var lowerDescription = description.ToLower();

            foreach (var category in categories)
            {
                if (category.Keywords.Any(keyword => lowerDescription.Contains(keyword.ToLower())))
                {
                    return category;
                }
            }
            return categories.FirstOrDefault(c => c.Type == TransactionType.Expense);
        }

        public async Task<List<Transaction>> GetRecurringTransactionsAsync()
        {
            return await _context.Transactions
                .Include( t => t.Category)
                .Where(t => t.IsRecurring)
                .OrderByDescending( t => t.Date)
                .ToListAsync();
        }

            public async Task GenerateRecurringTransactionsAsync()
        {
            var recurringTransactions = await _context.Transactions
                .Where(t => t.IsRecurring && t.RecurringType.HasValue)
                .ToListAsync();

            var today = DateTime.Today;

            foreach(var transaction in recurringTransactions)
            {
                if (transaction.RecurringEndDate.HasValue && transaction.RecurringEndDate < today)
                    continue;

                DateTime lastOccurrence = transaction.Date;
                bool shouldCreate = false;

                switch (transaction.RecurringType)
                {
                    case RecurringType.Daily:
                        shouldCreate = today > lastOccurrence.Date;
                        break;
                    case RecurringType.Weekly:
                        shouldCreate = today > lastOccurrence.AddDays(7);
                        break;
                    case RecurringType.Monthly:
                        shouldCreate = today > lastOccurrence.AddMonths(1);
                        break;
                    case RecurringType.Yearly:
                        shouldCreate = today > lastOccurrence.AddYears(1);
                        break;
                }

                if (shouldCreate)
                {
                    var newTransaction = new Transaction
                    {
                        Description = transaction.Description,
                        Amount = transaction.Amount,
                        Date = today,
                        Type = transaction.Type,
                        CategoryId = transaction.CategoryId,
                        IsRecurring = true,
                        RecurringType = transaction.RecurringType,
                        RecurringEndDate = transaction.RecurringEndDate,
                        Notes = transaction.Notes,
                    };

                    await CreateTransactionAsync(newTransaction);
                }
            }
        }

        public async Task<byte[]> ExportToCsvAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var transactions = await GetTransactionsAsync(startDate, endDate);
            var csv = "Date,Description,Category,Type,Amount,Notes\n";
            foreach(var transaction in transactions)
            {
                csv += $"\"{transaction.Date:yyyy-MM-dd}\",";
                csv += $"\"{transaction.Description}\",";
                csv += $"\"{transaction.Category?.Name}\",";
                csv += $"\"{transaction.Type}\",";
                csv += $"\"{transaction.Amount:C}\",";
                csv += $"\"{transaction.Notes}\"\n";
            }
            return System.Text.Encoding.UTF8.GetBytes(csv);
        }

        //CRUD operations
        public async Task<Transaction> GetTransactionAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task CreateTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        } 

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransactionAsync(int id)
        {
            var transaction = await GetTransactionAsync(id);
            if(transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }
    }

}
