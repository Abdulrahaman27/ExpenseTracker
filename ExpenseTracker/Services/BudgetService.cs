using ExpenseTracker.Data;
using ExpenseTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionService _transactionService;
        public BudgetService(ApplicationDbContext context, ITransactionService transactionService)
        {
            _context = context;
            _transactionService = transactionService;
        }

        public async Task<List<Budget>> GetBudgetsAsync()
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<Budget> GetBudgetAsync(int id)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task CreateBudgetAsync(Budget budget)
        {
            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBudgetAsync(Budget budget)
        {
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBudgetAsync(int id)
        {
            var budget = await GetBudgetAsync(id);
            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Budget>> GetBudgetsWithProgressAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var budgets = await GetBudgetsAsync();
            var transactions = await _transactionService.GetTransactionsAsync(startDate, endDate);

            foreach(var budget in budgets)
            {
                //calculate current spenfing for this budget category
                var categoryTransactions = transactions
                    .Where(t => t.CategoryId == budget.CategoryId &&
                    t.Date >= budget.StartDate && t.Date <= budget.EndDate)
                    .ToList();

                budget.CurrentSpending = categoryTransactions.Sum(t => t.Amount);
            }
            return budgets;
        }
    }
}
