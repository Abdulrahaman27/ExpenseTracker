using ExpenseTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Added this
using System.Text.Json; // Added for JsonSerializer

namespace ExpenseTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Budget> Budgets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring Transaction-category relationship
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuring Budget-category relationship
            modelBuilder.Entity<Budget>()
                .HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial data with JSON serialization
            // Note: Make sure your Category model has KeywordsJson property
            var foodKeywords = JsonSerializer.Serialize(new List<string>
            { "restaurant", "food", "dinner", "lunch", "coffee", "grocery" });

            var transportKeywords = JsonSerializer.Serialize(new List<string>
            { "gas", "fuel", "uber", "taxi", "bus", "train", "metro" });

            var shoppingKeywords = JsonSerializer.Serialize(new List<string>
            { "shopping", "clothes", "electronics", "amazon", "store" });

            var entertainmentKeywords = JsonSerializer.Serialize(new List<string>
            { "movie", "netflix", "concert", "game", "entertainment" });

            var salaryKeywords = JsonSerializer.Serialize(new List<string>
            { "salary", "paycheck", "income" });

            var freelanceKeywords = JsonSerializer.Serialize(new List<string>
            { "freelance", "contract", "project" });

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Food & Dining",
                    Icon = "bi-cup-straw",
                    Color = "#dc3545",
                    Type = TransactionType.Expense,
                    KeywordsJson = foodKeywords
                },
                new Category
                {
                    Id = 2,
                    Name = "Transportation",
                    Icon = "bi-car-front",
                    Color = "#007bff",
                    Type = TransactionType.Expense,
                    KeywordsJson = transportKeywords
                },
                new Category
                {
                    Id = 3,
                    Name = "Shopping",
                    Icon = "bi-bag",
                    Color = "#ffc107",
                    Type = TransactionType.Expense,
                    KeywordsJson = shoppingKeywords
                },
                new Category
                {
                    Id = 4,
                    Name = "Entertainment",
                    Icon = "bi-film",
                    Color = "#20c997",
                    Type = TransactionType.Expense,
                    KeywordsJson = entertainmentKeywords
                },
                new Category
                {
                    Id = 5,
                    Name = "Salary",
                    Icon = "bi-cash-stack",
                    Color = "#28a745",
                    Type = TransactionType.Income,
                    KeywordsJson = salaryKeywords
                },
                new Category
                {
                    Id = 6,
                    Name = "Freelance",
                    Icon = "bi-laptop",
                    Color = "#28a745",
                    Type = TransactionType.Income,
                    KeywordsJson = freelanceKeywords
                }
            );
        }
    }
}