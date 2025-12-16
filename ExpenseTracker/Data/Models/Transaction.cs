using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Data.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")] // Add this
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public bool IsRecurring { get; set; }
        public RecurringType? RecurringType { get; set; }
        public DateTime? RecurringEndDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public enum TransactionType
    {
        Expense = 0,
        Income = 1
    }

    public enum RecurringType
    {
        Daily = 0,
        Weekly = 1,
        Monthly = 2,
        Yearly = 3
    }
}