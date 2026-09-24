# Personal Expense Tracker

A full-stack web application built with **ASP.NET Core MVC**, **Entity Framework Core** and **SQL Server**
that lets users register, log in, record income and expenses, organize transactions by category, set
monthly budgets, and view spending reports. Built as a BCA / B.Sc. IT college mini project.

> ⚠️ **Build verification notice:** This project was generated in an environment with **no .NET SDK
> installed and no internet/NuGet access**, so the code could not be compiled, migrated or run here.
> Every file below is complete, hand-written C#/Razor code following standard ASP.NET Core MVC + EF
> Core patterns, but you **must** build and run it locally (or in Visual Studio) using the steps in
> this README to confirm it compiles cleanly on your machine. If you hit a build error, it is most
> likely a small typo or a NuGet package version mismatch — check `PersonalExpenseTracker.csproj`
> first.

---

## 1. Project Title

**Personal Expense Tracker**

## 2. Project Description

Personal Expense Tracker helps individuals track where their money comes from and where it goes. Users
register an account, log in securely, and then add income and expense entries tagged with a category,
date and payment method. The dashboard shows total income, total expenses, current balance and recent
activity. A budgeting module lets users cap monthly spending per category with visual progress bars, and
the reports module shows category-wise spending and month-over-month income vs. expense using Chart.js.
An admin panel lets an administrator oversee all registered users, categories and transactions.

## 3. Features

- User registration & secure login (hashed passwords, no plain text ever stored)
- Personal dashboard with income, expenses, balance, monthly spend and recent transactions
- Full CRUD for Expenses, Income, Categories and Budgets
- Combined Transactions view with search, filter and sort
- Search & filter expenses/income by description, category, date range, payment method, amount range
- Monthly Summary report (income, expenses, balance, highest/average expense)
- Expense Report with Chart.js pie chart (by category) and bar chart (income vs expense, 6 months)
- Budget module with progress bars and "budget exceeded" / "% used" warnings
- Profile management (edit details, change password)
- Separate Admin login and Admin Dashboard (manage users, categories, transactions, view reports)
- Role-based authorization (`[Authorize(Roles = "User")]` / `[Authorize(Roles = "Admin")]`)
- Anti-forgery tokens on all POST actions
- Users can only ever see/edit/delete their own data
- Friendly 404 and error pages
- Responsive Bootstrap 5 UI with a clean green/blue color scheme

## 4. Technology Used

- ASP.NET Core MVC (.NET 8 LTS)
- C#
- Entity Framework Core 8 (Code First) + SQL Server / LocalDB
- Razor Views
- Bootstrap 5 + Bootstrap Icons
- Chart.js (via CDN)
- Cookie-based authentication (`Microsoft.AspNetCore.Authentication.Cookies`)

## 5. Software Requirements

- Visual Studio 2022 (17.8+) **or** VS Code with the C# Dev Kit extension
- .NET 8 SDK — https://dotnet.microsoft.com/download/dotnet/8.0
- SQL Server LocalDB (installed automatically with Visual Studio's "ASP.NET and web development"
  workload) — or any SQL Server instance
- EF Core CLI tools (see section 10)

## 6. How to Open the Project

1. Extract `PersonalExpenseTracker.zip`.
2. Open the folder in Visual Studio (`File → Open → Folder`) or double-click
   `PersonalExpenseTracker.csproj`, or open the folder in VS Code.

## 7. How to Configure SQL Server

By default the project uses **SQL Server LocalDB**, which ships with Visual Studio. No extra
installation is normally required. If you prefer a full SQL Server instance (e.g. SQL Server Express or
a Docker container), update the connection string as shown below.

## 8. How to Update the Connection String

Open `appsettings.json` and edit the `DefaultConnection` value:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PersonalExpenseTrackerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

Example for a named SQL Server instance with SQL authentication:

```json
"DefaultConnection": "Server=YOUR_SERVER;Database=PersonalExpenseTrackerDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
```

## 9. How to Create the Database

The app calls `context.Database.Migrate()` on startup, so once a migration exists, running the app
creates/updates the database automatically. To generate the migration yourself:

```bash
dotnet ef migrations add InitialCreate
```

## 10. How to Run Migrations

```bash
dotnet restore
dotnet build
dotnet ef migrations add InitialCreate
dotnet ef database update
```

If `dotnet ef` is not recognized, install the EF Core CLI tool once per machine:

```bash
dotnet tool install --global dotnet-ef
```

Then re-run the migration commands above. (You may need to restart your terminal after installing the
tool so PATH updates take effect.)

## 11. How to Start the Application

```bash
dotnet run
```

Or press **F5** / **Ctrl+F5** in Visual Studio. The app will open in your browser (typically
`https://localhost:5001` — check the console output for the exact port).

On first run, `DbSeeder.Seed()` automatically inserts:
- 1 admin account
- 10 default expense categories + 6 default income categories
- 10 sample users
- Sample income, expense and budget records with realistic Indian amounts for the first 3 sample users

## 12. Demo Admin Login

```
URL:      /Account/AdminLogin
Email:    admin@gmail.com
Password: Admin@123
```

**These are demo credentials for college project demonstration only.**

## 13. Sample User Login

Every seeded sample user shares the same demo password:

```
Email:    aarav.sharma@example.com   (or any of the 10 seeded users — see Data/DbSeeder.cs)
Password: Password@123
```

You can also simply register your own account from the **Register** page.

## 14. Project Folder Structure

```
PersonalExpenseTracker/
├── Controllers/
│   ├── HomeController.cs
│   ├── AccountController.cs
│   ├── DashboardController.cs
│   ├── ExpenseController.cs
│   ├── IncomeController.cs
│   ├── CategoryController.cs
│   ├── BudgetController.cs
│   ├── TransactionController.cs
│   ├── ReportController.cs
│   ├── ProfileController.cs
│   └── AdminController.cs
├── Models/
│   ├── User.cs, Admin.cs, Category.cs, Expense.cs, Income.cs, Budget.cs
├── Data/
│   ├── ApplicationDbContext.cs
│   └── DbSeeder.cs
├── ViewModels/
│   ├── LoginViewModel.cs, RegisterViewModel.cs, DashboardViewModel.cs
│   ├── ExpenseViewModel.cs, IncomeViewModel.cs, BudgetViewModel.cs
│   ├── ReportViewModel.cs, ProfileViewModel.cs
├── Helpers/
│   ├── PasswordHasher.cs, AuthConstants.cs, CurrentUser.cs
├── Views/
│   ├── Home/, Account/, Dashboard/, Expense/, Income/, Category/,
│   │   Budget/, Transaction/, Report/, Profile/, Admin/, Shared/
├── wwwroot/
│   ├── css/site.css
│   └── js/site.js, dashboard.js, reports.js
├── Documentation/
│   ├── Project_Report.md, Database_Schema.md, Project_Explanation.md, Testing.md
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
└── PersonalExpenseTracker.csproj
```

## 15. Database Structure

See `Documentation/Database_Schema.md` for the full table/column/relationship breakdown and a simple
text ER diagram.

## 16. How Authentication Works

The app uses **cookie authentication** (`Microsoft.AspNetCore.Authentication.Cookies`). On successful
login, a `ClaimsPrincipal` is built with the user's/admin's ID, name, email and a `Role` claim
(`"User"` or `"Admin"`), then signed into an encrypted cookie via `HttpContext.SignInAsync`. Controllers
are protected with `[Authorize(Roles = "User")]` or `[Authorize(Roles = "Admin")]`. Passwords are never
stored in plain text — they're hashed with PBKDF2 (`Rfc2898DeriveBytes`, 100,000 iterations, random salt
per user) in `Helpers/PasswordHasher.cs`.

## 17. How Expense Tracking Works

Users add an expense with amount, category, description, date, payment method and optional notes via
`ExpenseController`. All queries are scoped to `WHERE UserId == <current user>` so nobody can see or
modify another user's data. The Expense list page supports search, category/date/payment/amount
filters, and sorting.

## 18. How Budget Tracking Works

A budget is created per category, per month/year (`BudgetController`). On the Budget Index page, the
"spent" amount is calculated live from the `Expenses` table for that category/month, and a Bootstrap
progress bar shows percentage used, with a warning badge at 90%+ and "Budget exceeded" once spending
passes 100%.

## 19. Future Improvements

- Export reports to PDF/Excel
- Recurring transactions (e.g. auto-add monthly rent)
- Email notifications when a budget is close to being exceeded
- Multi-currency support
- Two-factor authentication
- Pagination on large transaction lists
