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

        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")] // Add this
        public decimal Amount { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        [Required]
        public DateTime EndDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);

        [StringLength(200)]
        public string? Description { get; set; }

        // Navigation property for easy access
        [Column(TypeName = "decimal(18,2)")] // Add this
        public decimal CurrentSpending { get; set; }

        [NotMapped]
        public decimal ProgressPercentage => Amount > 0 ? (CurrentSpending / Amount) * 100 : 0;
    }
}