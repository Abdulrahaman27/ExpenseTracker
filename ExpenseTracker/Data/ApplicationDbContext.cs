using ExpenseTracker.Data.Models;
using Microsoft.EntityFrameworkCore;
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

            //Configuring Transaction-category relationship
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c  => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            //Configuring Budget-category relationship
            modelBuilder.Entity<Budget>()
                .HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial data
            modelBuilder.Entity<Category>().HasData(
                new Category 
                { 
                    Id = 1,
                    Name = "Food & Dining",
                    Icon = "bi-cup-straw",
                    Color = "#dc3545",
                    Type = TransactionType.Expense,
                    Keywords = new List<string> { "restaurant", "food", "dinner", "lunch", "coffee", "grocery" } },
                new Category
                {
                    Id = 2,
                    Name = "Transportation",
                    Icon = "bi-car-front",
                    Color = "#007bff",
                    Type = TransactionType.Expense,
                    Keywords = new List<string> { "gas", "fuel", "uber", "taxi", "bus", "train", "metro" }
                },
                new Category
                {
                    Id = 3,
                    Name = "Shopping",
                    Icon = "bi-bag",
                    Color = "#ffc107",
                    Type = TransactionType.Expense,
                    Keywords = new List<string> { "shopping", "clothes", "electronics", "amazon", "store" }
                },
                new Category
                {
                    Id = 4,
                    Name = "Entertainment",
                    Icon = "bi-film",
                    Color = "#20c997",
                    Type = TransactionType.Expense,
                    Keywords = new List<string> { "movie", "netflix", "concert", "game", "entertainment" }
                },
                 new Category
                 {
                     Id = 5,
                     Name = "Salary",
                     Icon = "bi-cash-stack",
                     Color = "#28a745",
                     Type = TransactionType.Income,
                     Keywords = new List<string> { "salary", "paycheck", "income" }
                 },
                new Category
                {
                    Id = 6,
                    Name = "Freelance",
                    Icon = "bi-laptop",
                    Color = "#28a745",
                    Type = TransactionType.Income,
                    Keywords = new List<string> { "freelance", "contract", "project" }
                }
                );

        }
    }
}
