using ExpenseTracker.Data;
using ExpenseTracker.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Db context
builder.Services.AddDbContext<ApplicationDbContext>(options
     => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adding services
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Ensure database is migrated
    dbContext.Database.Migrate();

    // Call SeedData function
    SeedData(dbContext);
}

app.Run();

// SeedData function
void SeedData(ApplicationDbContext context)
{
    // Only seed if no categories exist
    if (!context.Categories.Any())
    {
        // First, add ALL categories with their IDs
        var categories = new[]
        {
            new ExpenseTracker.Data.Models.Category
            {
                Id = 1,
                Name = "Food & Dining",
                Icon = "bi-cup-straw",
                Color = "#dc3545",
                Type = ExpenseTracker.Data.Models.TransactionType.Expense,
                Description = "Food expenses"
            },
            new ExpenseTracker.Data.Models.Category
            {
                Id = 2,
                Name = "Transportation",
                Icon = "bi-car-front",
                Color = "#007bff",
                Type = ExpenseTracker.Data.Models.TransactionType.Expense,
                Description = "Transportation costs"
            },
            new ExpenseTracker.Data.Models.Category
            {
                Id = 3,
                Name = "Shopping",
                Icon = "bi-bag",
                Color = "#ffc107",
                Type = ExpenseTracker.Data.Models.TransactionType.Expense,
                Description = "Shopping expenses"
            },
            new ExpenseTracker.Data.Models.Category
            {
                Id = 4,
                Name = "Entertainment",
                Icon = "bi-film",
                Color = "#20c997",
                Type = ExpenseTracker.Data.Models.TransactionType.Expense,
                Description = "Entertainment expenses"
            },
            new ExpenseTracker.Data.Models.Category
            {
                Id = 5,
                Name = "Salary",
                Icon = "bi-cash-stack",
                Color = "#28a745",
                Type = ExpenseTracker.Data.Models.TransactionType.Income,
                Description = "Salary income"
            },
            new ExpenseTracker.Data.Models.Category
            {
                Id = 6,
                Name = "Freelance",
                Icon = "bi-laptop",
                Color = "#28a745",
                Type = ExpenseTracker.Data.Models.TransactionType.Income,
                Description = "Freelance income"
            }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        Console.WriteLine("Added categories to database.");

        // Now add transactions using the existing category IDs
        var transactions = new[]
        {
            new ExpenseTracker.Data.Models.Transaction
            {
                Description = "Grocery Shopping",
                Amount = 150.50m,
                Date = DateTime.Now.AddDays(-1),
                Type = ExpenseTracker.Data.Models.TransactionType.Expense,
                CategoryId = 1  // Food & Dining
            },
            new ExpenseTracker.Data.Models.Transaction
            {
                Description = "Monthly Salary",
                Amount = 3000.00m,
                Date = DateTime.Now.AddDays(-5),
                Type = ExpenseTracker.Data.Models.TransactionType.Income,
                CategoryId = 5  // Salary
            },
            new ExpenseTracker.Data.Models.Transaction
            {
                Description = "Gas Station",
                Amount = 45.75m,
                Date = DateTime.Now.AddDays(-2),
                Type = ExpenseTracker.Data.Models.TransactionType.Expense,
                CategoryId = 2  // Transportation
            },
            new ExpenseTracker.Data.Models.Transaction
            {
                Description = "Netflix Subscription",
                Amount = 15.99m,
                Date = DateTime.Now.AddDays(-3),
                Type = ExpenseTracker.Data.Models.TransactionType.Expense,
                CategoryId = 4  // Entertainment
            },
            new ExpenseTracker.Data.Models.Transaction
            {
                Description = "Freelance Project",
                Amount = 500.00m,
                Date = DateTime.Now.AddDays(-7),
                Type = ExpenseTracker.Data.Models.TransactionType.Income,
                CategoryId = 6  // Freelance
            }
        };

        context.Transactions.AddRange(transactions);
        context.SaveChanges();

        Console.WriteLine($"Added {transactions.Length} test transactions.");
    }
    else
    {
        Console.WriteLine("Database already has data. Skipping seeding.");
    }
}