using ExpenseTracker.Data.Models;

namespace ExpenseTracker.Services
{
    public interface IBudgetService
    {
        Task<List<Budget>> GetBudgetsAsync();
        Task<Budget> GetBudgetAsync(int id);
        Task CreateBudgetAsync(Budget budget);
        Task UpdateBudgetAsync(Budget budget);
        Task DeleteBudgetAsync(int id);
        Task<Budget> GetBudgetWithSpendingAsync(int id);
        Task<List<Budget>> GetActiveBudgetsAsync();
        Task<decimal> CalculateSpentAmountAsync(int budgetId, DateTime? startDate = null, DateTime? endDate = null);

        Task<List<Budget>> GetBudgetsWithProgressAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
