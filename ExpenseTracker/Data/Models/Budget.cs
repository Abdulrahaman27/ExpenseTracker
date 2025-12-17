using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Data.Models
{
    public class Budget
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        // Track spending (calculated, not stored)
        [NotMapped]
        public decimal CurrentSpending { get; set; }

        [NotMapped]
        public decimal RemainingAmount => Amount - CurrentSpending;

        [NotMapped]
        public decimal PercentageUsed => Amount > 0 ? (CurrentSpending / Amount) * 100 : 0;

        [NotMapped]
        public BudgetStatusType Status // Renamed to avoid conflict
        {
            get
            {
                if (CurrentSpending > Amount) return BudgetStatusType.Exceeded;
                if (PercentageUsed >= WarningThreshold) return BudgetStatusType.Warning; // Use WarningThreshold property
                return BudgetStatusType.OnTrack;
            }
        }

        [Required]
        public BudgetPeriod Period { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? LastResetDate { get; set; }
        public DateTime? NextResetDate { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public bool NotifyOnExceed { get; set; } = true;
        public bool NotifyOnWarning { get; set; } = true;
        public int WarningThreshold { get; set; } = 80;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }

    // Renamed to avoid conflict with enum
    public class BudgetStatusSummary
    {
        public string BudgetName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal BudgetAmount { get; set; }
        public decimal ActualSpending { get; set; }
        public decimal Difference => BudgetAmount - ActualSpending;
        public bool IsOverBudget => ActualSpending > BudgetAmount;
    }

    // Renamed to avoid naming conflict
    public enum BudgetStatusType
    {
        OnTrack = 0,
        Warning = 1,
        Exceeded = 2
    }

    public enum BudgetPeriod
    {
        Daily = 0,
        Weekly = 1,
        Monthly = 2,
        Quarterly = 3,
        Yearly = 4,
        Custom = 5
    }

    public class BudgetNotification
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public Budget Budget { get; set; } = null!;
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public enum NotificationType
    {
        BudgetExceeded = 0,
        BudgetWarning = 1,
        BudgetReset = 2
    }

    // Assuming this exists somewhere (was referenced but not defined)
    //public class Category
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //}
}