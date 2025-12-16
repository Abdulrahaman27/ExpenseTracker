using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ExpenseTracker.Data.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Icon { get; set; } = "bi-tag";

        [Required]
        public string Color { get; set; } = "#6c757d";
        
        [Required]
        public TransactionType Type { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

        //For smart suggestions - store as JSON in database
        [Column(TypeName = "nvarchar(max)")]
        public string KeywordsJson { get; set; }

        [NotMapped]
        public List<string> Keywords
        {
            get => string.IsNullOrEmpty(KeywordsJson) ? new List<string>(): System.Text.Json.JsonSerializer.Deserialize<List<string>>(KeywordsJson);
            set => KeywordsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }
    }
}
