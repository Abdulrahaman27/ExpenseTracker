using ExpenseTracker.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    public interface IBudgetService
    {
        Task<List<Budget>> GetBudgetsAsync();
        Task<Budget> GetBudgetAsync(int budgetId);  
        Task CreateBudgetAsync(Budget budget);
        Task UpdateBudgetAsync(Budget budget);
        Task DeleteBudgetAsync(int id);
        Task<List<Budget>> GetBudgetsWithProgressAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
