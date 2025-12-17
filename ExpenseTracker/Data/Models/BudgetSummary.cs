using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Data.Models
{
    public class BudgetSummary
    {
        public int TotalBudgets { get; set; }
        public int ActiveBudgets { get; set; }
        public int OnTrackBudgets { get; set; }
        public int WarningBudgets { get; set; }
        public int ExceededBudgets { get; set; }
        public decimal TotalBudgetedAmount { get; set; }
        public decimal TotalCurrentSpending { get; set; }
        public decimal TotalRemainingAmount => TotalBudgetedAmount - TotalCurrentSpending;
        public decimal OverallBudgetUsage => TotalBudgetedAmount > 0 ? (TotalCurrentSpending / TotalBudgetedAmount) * 100 : 0;
    }
}
