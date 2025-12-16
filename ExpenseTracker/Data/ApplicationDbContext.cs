using ExpenseTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;

namespace ExpenseTracker.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

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

            // FIX: Configure decimal precision for SQL Server
            // For Transaction Amount
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2); // 18 total digits, 2 decimal places

            // For Budget Amount and CurrentSpending
            modelBuilder.Entity<Budget>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Budget>()
                .Property(b => b.CurrentSpending)
                .HasPrecision(18, 2);

            // Seed initial data with JSON serialization
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
                    Description = "Restaurants, groceries, coffee shops",
                    KeywordsJson = foodKeywords
                },
                new Category
                {
                    Id = 2,
                    Name = "Transportation",
                    Icon = "bi-car-front",
                    Color = "#007bff",
                    Type = TransactionType.Expense,
                    Description = "Gas, public transport, ride sharing",
                    KeywordsJson = transportKeywords
                },
                new Category
                {
                    Id = 3,
                    Name = "Shopping",
                    Icon = "bi-bag",
                    Color = "#ffc107",
                    Type = TransactionType.Expense,
                    Description = "Clothing, electronics, household items",
                    KeywordsJson = shoppingKeywords
                },
                new Category
                {
                    Id = 4,
                    Name = "Entertainment",
                    Icon = "bi-film",
                    Color = "#20c997",
                    Type = TransactionType.Expense,
                    Description = "Movies, streaming services, concerts",
                    KeywordsJson = entertainmentKeywords
                },
                new Category
                {
                    Id = 5,
                    Name = "Salary",
                    Icon = "bi-cash-stack",
                    Color = "#28a745",
                    Type = TransactionType.Income,
                    Description = "Regular employment income",
                    KeywordsJson = salaryKeywords
                },
                new Category
                {
                    Id = 6,
                    Name = "Freelance",
                    Icon = "bi-laptop",
                    Color = "#28a745",
                    Type = TransactionType.Income,
                    Description = "Freelance work and contract income",
                    KeywordsJson = freelanceKeywords
                }
            );
        }
    }
}