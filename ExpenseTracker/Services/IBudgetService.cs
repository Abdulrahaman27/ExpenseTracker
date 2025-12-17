using ExpenseTracker.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTracker.Services
{
    public interface IBudgetService
    {
        // Basic CRUD operations
        Task<List<Budget>> GetBudgetsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<Budget> GetBudgetAsync(int id);
        Task<Budget> GetBudgetWithSpendingAsync(int id);
        Task CreateBudgetAsync(Budget budget);
        Task UpdateBudgetAsync(Budget budget);
        Task DeleteBudgetAsync(int id);

        // Budget tracking and calculation
        Task<List<Budget>> GetActiveBudgetsAsync();
        Task<decimal> CalculateSpentAmountAsync(int budgetId, DateTime? startDate = null, DateTime? endDate = null);

        // Enhanced features for tracking
        Task CheckBudgetsAgainstTransaction(Transaction transaction);
        Task ResetExpiredBudgets();
        Task<List<Budget>> GetBudgetsRequiringAttentionAsync();
        Task<List<BudgetNotification>> GetBudgetNotificationsAsync(int? budgetId = null, bool unreadOnly = false);
        Task MarkNotificationAsReadAsync(int notificationId);
        Task MarkAllNotificationsAsReadAsync();

        // For reports
        Task<List<Budget>> GetBudgetsWithSpendingAsync(DateTime? startDate = null, DateTime? endDate = null);

        // Summary and analysis
        Task<BudgetSummary> GetBudgetSummaryAsync();
        Task<List<BudgetNotification>> GetRecentNotificationsAsync(int count = 10);

        Task<List<Budget>> GetBudgetsWithProgressAsync(DateTime? startDate = null, DateTime? endDate = null);
    }

}