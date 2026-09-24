# Project Explanation (Viva Preparation Guide)

## What is this project?
Personal Expense Tracker is a web application that lets a person track how much money they earn
(income) and spend (expenses), organize those transactions into categories, set monthly budgets, and
see visual reports of their spending. It is built with ASP.NET Core MVC, Entity Framework Core, and
SQL Server.

## Why is it required?
Many people don't have a simple, dedicated way to see where their money goes each month. Spreadsheets
require manual formulas; paper tracking is error-prone. This app automatically calculates totals,
balances, and budget usage, and shows the data visually through charts.

## How registration works
1. The user fills the Register form (Full Name, Email, Password, Confirm Password, Phone Number).
2. `RegisterViewModel` uses Data Annotations (`[Required]`, `[EmailAddress]`, `[Compare]`, etc.) for
   client- and server-side validation.
3. `AccountController.Register()` checks the email isn't already used, hashes the password with
   `PasswordHasher.HashPassword()`, and saves a new `User` row via EF Core.
4. The user is redirected to the Login page with a success message.

## How login works
1. The user enters email and password on the Login form.
2. `AccountController.Login()` looks up the user by email, then verifies the password using
   `PasswordHasher.VerifyPassword()`, which re-computes the PBKDF2 hash with the stored salt and
   compares it to the stored hash using a constant-time comparison.
3. If valid, a cookie-based authentication ticket is issued with claims for UserId, Name, Email and
   Role = "User".

## How authentication works
The app uses ASP.NET Core's built-in Cookie Authentication. After a successful login,
`HttpContext.SignInAsync()` writes an encrypted authentication cookie containing the user's claims.
Every subsequent request automatically has `User.Identity` populated from that cookie. Controllers use
`[Authorize(Roles = "User")]` or `[Authorize(Roles = "Admin")]` to restrict access. `Logout()` calls
`HttpContext.SignOutAsync()` to clear the cookie.

## How expenses are added
The user goes to Expenses → Add Expense, fills in amount, category, description, date, payment method
and optional notes. `ExpenseController.Create()` validates the model, attaches the currently logged-in
`UserId` (read from claims via `CurrentUser.GetUserId(User)`), and saves it with EF Core. The Expense
List page then re-queries only that user's expenses (`Where(e => e.UserId == userId)`), so users never
see each other's data.

## How income is added
Same pattern as expenses, but through `IncomeController`, with fields for Source and Category (income
categories like Salary, Freelance, etc.) instead of Payment Method.

## How categories work
Categories can be **system defaults** (seeded once, with `UserId = null`, visible to everyone) or
**user-created** (with `UserId` set to that user, visible only to them). A category cannot be deleted
if it's already used by an existing Expense, Income, or Budget row — the controller checks this with
`AnyAsync()` before allowing deletion.

## How budgets work
A user picks a category and sets a maximum amount for a given month/year. On the Budget page, the
"spent so far" amount is computed live: `SUM(Expenses.Amount)` for that user, category, month and year.
The percentage used is `(spent / budget) * 100`, shown with a Bootstrap progress bar that turns
yellow at 75%+ and red at 100%+ ("Budget exceeded").

## How dashboard calculations work
Nothing on the dashboard is hard-coded. `DashboardController.Index()` runs EF Core aggregate queries:
- `TotalIncome` = `SUM(Incomes.Amount)` for the user
- `TotalExpenses` = `SUM(Expenses.Amount)` for the user
- `Balance` = `TotalIncome - TotalExpenses` (computed property)
- `CurrentMonthExpenses` = `SUM(Expenses.Amount)` where the expense date is in the current month/year
- `TransactionCount` = income count + expense count
- `RecentTransactions` = the 5 most recent income + expense rows combined and sorted by date

## How reports work
`ReportController.MonthlySummary()` totals income/expenses for a chosen month/year and computes
highest/average expense. `ReportController.ExpenseReport()` groups expenses by category (for the pie
chart) and computes income/expense totals for the last 6 months (for the bar chart). The controller
serializes this data to JSON and passes it to the view, where `reports.js` renders it with Chart.js —
so charts always reflect live database data, never hard-coded numbers.

## How Entity Framework works (in this project)
`ApplicationDbContext` (in `Data/ApplicationDbContext.cs`) defines `DbSet<T>` properties for each
entity. Relationships and delete behaviors are configured in `OnModelCreating()`. On app startup,
`context.Database.Migrate()` applies any pending EF Core migrations, and `DbSeeder.Seed()` inserts
demo data if the tables are empty. Controllers use `_context.Expenses.Where(...)`, `.Include(...)`,
`.SumAsync(...)` etc. — standard EF Core LINQ — to query and update data asynchronously.

## How SQL Server is connected
The connection string lives in `appsettings.json` under `ConnectionStrings:DefaultConnection` and
points at SQL Server LocalDB by default. `Program.cs` registers the DbContext with
`options.UseSqlServer(connectionString)`, so EF Core knows to generate T-SQL and talk to SQL Server.

## How admin works
An `Admin` account (separate table from `User`) is seeded once with the demo credentials
(`admin@gmail.com` / `Admin@123`). Admin login uses the same cookie authentication scheme but sets
`Role = "Admin"` instead of `"User"`. `AdminController` (protected with
`[Authorize(Roles = "Admin")]`) exposes a dashboard with system-wide statistics, and pages to view all
users, manage categories, and view/delete any transaction in the system.
