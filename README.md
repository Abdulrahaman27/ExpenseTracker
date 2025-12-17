# 💰 ExpenseTracker

A modern, feature-rich personal finance management application built with ASP.NET Core and Entity Framework Core. Track expenses, manage budgets, and gain insights into your spending habits.

## 🚀 Features

### 📊 Core Functionality
- **Expense & Income Tracking** - Record and categorize all financial transactions
- **Budget Management** - Set spending limits with customizable periods
- **Financial Reporting** - Generate detailed reports and insights
- **Category Management** - Organize transactions with custom categories

### 🔔 Smart Features
- **Budget Notifications** - Get alerts when approaching or exceeding limits
- **Automated Resets** - Budgets automatically reset based on chosen periods
- **Progress Tracking** - Visual indicators for budget usage and progress
- **Monthly/Yearly Summaries** - View spending trends over time

## 🏗️ Technology Stack

- **Backend**: ASP.NET Core 7.0, Entity Framework Core
- **Frontend**: Razor Pages, Bootstrap 5, JavaScript
- **Database**: SQL Server (with migrations support)
- **Architecture**: Repository Pattern, Service Layer

## 📁 Project Structure
ExpenseTracker/
├── Pages/ # Razor Pages
│ ├── Budgets/ # Budget management pages
│ ├── Transactions/ # Transaction pages
│ ├── Reports/ # Financial reports
│ └── Categories/ # Category management
├── Services/ # Business logic layer
├── Data/ # Data access layer
├── Models/ # Domain models
└── Migrations/ # Database migrations
## ⚙️ Setup & Installation

### Prerequisites
- [.NET 7.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/Abdulrahaman27/ExpenseTracker.git
   cd ExpenseTracker
   Configure database connection

Update the connection string in appsettings.json:

json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ExpenseTracker;Trusted_Connection=True;"
}
Apply database migrations

bash
dotnet ef database update
Run the application

bash
dotnet run
Navigate to https://localhost:5001

📋 Key Models
Model	Description
Transaction	Income/Expense records with categorization
Budget	Spending limits with period tracking
Category	Transaction categories with color coding
BudgetNotification	Alert system for budget status
🔧 Service Layer
IBudgetService - Budget CRUD operations and tracking

ITransactionService - Transaction management

IReportService - Financial reporting and analysis

ICategoryService - Category management

📊 Reports Available
Monthly Report - Income/Expense breakdown by month

Yearly Report - Annual financial summary

Custom Report - Date-range specific analysis

Category Report - Spending by category

Budget Performance - Budget vs actual spending

🎨 UI Features
Responsive design (works on mobile & desktop)

Interactive charts and progress bars

Real-time budget status indicators

Toast notifications for user actions

Dark/light mode support (via Bootstrap)

🗃️ Database Migrations
The project uses EF Core Code-First migrations:

# Add a new migration
dotnet ef migrations add MigrationName

# Apply migrations to database
dotnet ef database update

# List all migrations
dotnet ef migrations list

🔐 Security Features
Input validation and sanitization

Anti-forgery tokens on forms

SQL injection prevention (parameterized queries)

XSS protection

🚦 Running Tests
# Run unit tests
dotnet test

🤝 Contributing
Fork the repository

Create a feature branch (git checkout -b feature/AmazingFeature)

Commit changes (git commit -m 'Add some AmazingFeature')

Push to branch (git push origin feature/AmazingFeature)

Open a Pull Request

📄 License
This project is licensed under the MIT License - see the LICENSE file for details.

🙏 Acknowledgments
Bootstrap for the responsive UI components

Font Awesome for icons

Chart.js for data visualization

All contributors who have helped shape this project

# Support
For questions, issues, or feature requests:

Check the Issues page

Create a new issue with detailed description
