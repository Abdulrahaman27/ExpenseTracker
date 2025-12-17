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
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentSpending { get; set; }

        [Required]
        public BudgetPeriod Period { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }


        //Optional
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        [NotMapped]
        public decimal SpentAmount { get; set; }

        [NotMapped]
        public decimal RemainingAmount => Amount - SpentAmount;

        [NotMapped]
        public decimal PercentageUsed => Amount > 0 ? (SpentAmount / Amount) * 100 : 0;

        [NotMapped]
        public bool IsOverBudget => SpentAmount > Amount;

        // Audit fields
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
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
}