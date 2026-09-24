# Project Report: Personal Expense Tracker

## 1. Introduction
Personal Expense Tracker is a web-based application that allows individuals to record their daily
income and expenses, organize them into categories, plan monthly budgets, and view simple visual
reports of their spending habits. It is built using ASP.NET Core MVC with Entity Framework Core and
SQL Server as part of a BCA/B.Sc. IT college mini project.

## 2. Problem Statement
Most people track their expenses manually on paper or in scattered notes/spreadsheets, which makes it
hard to know exactly how much they earn, spend, and save each month. There is no easy way to see
spending patterns by category or to know when a budget is close to being exceeded. This project solves
that by providing a simple, dedicated web application for personal finance tracking.

## 3. Objectives
- Allow users to securely register and log in.
- Let users record income and expense transactions with category, date, and payment method.
- Provide a dashboard summarizing income, expenses, balance and monthly spending.
- Allow users to set monthly budgets per category and track usage.
- Provide visual reports (charts) of spending by category and income vs. expense trends.
- Provide an admin panel to oversee users, categories and transactions.

## 4. Scope
The system covers individual personal finance tracking: registration, login, income/expense
management, categorization, budgeting, reporting, and basic admin oversight. It does not cover bank
account integration, multi-currency conversion, or tax filing.

## 5. Existing System
Most existing solutions are either generic spreadsheet templates (manual, error-prone, no automatic
calculations) or large commercial finance apps with many unrelated features, subscriptions, and bank
linking that are overkill for a simple personal tracker or a student project demonstration.

## 6. Proposed System
The proposed system is a lightweight, purpose-built web application where a user can log every income
and expense in seconds, see automatically calculated totals and balances, and get visual feedback on
their spending through category charts and budget progress bars — all without manual calculation.

## 7. Functional Requirements
- User registration and login with validation
- CRUD operations for Income, Expense, Category, and Budget
- Search and filter on transactions
- Monthly summary and category-wise expense report
- Budget progress tracking with alerts
- Admin login, dashboard, and management of users/categories/transactions

## 8. Non-Functional Requirements
- Security: hashed passwords, anti-forgery tokens, per-user data isolation
- Usability: responsive Bootstrap UI, clear validation messages
- Performance: EF Core queries scoped and indexed on Email
- Maintainability: clean MVC separation of concerns (Controllers/Models/Views/ViewModels)

## 9. System Modules
### 9.1 User Module
Handles registration, login, logout, and profile management.

### 9.2 Expense Module
Add/edit/delete/view expenses with category, amount, date, payment method and notes.

### 9.3 Income Module
Add/edit/delete/view income entries with source, category, amount, and date.

### 9.4 Category Module
Manage income/expense categories; includes system default categories and user-created ones.

### 9.5 Budget Module
Set a monthly budget per category; track spent vs. remaining with a progress bar.

### 9.6 Dashboard Module
Shows total income, total expenses, balance, this month's expenses, transaction count, and recent
transactions — all calculated live from the database.

### 9.7 Report Module
Monthly summary (income, expenses, balance, highest/average expense) and a category-wise expense
report with Chart.js pie and bar charts.

### 9.8 Admin Module
Admin dashboard with system-wide statistics, and management pages for users, categories and
transactions.

## 10. Database Design
See `Database_Schema.md` for full details, including tables, columns, keys, and a text ER diagram.

## 11. ER Diagram Description
A `User` can have many `Expense`, `Income`, `Budget`, and `Category` records. A `Category` can be
referenced by many `Expense`, `Income`, and `Budget` records. An `Admin` account is separate from
`User` and is used only for administrative login and management actions.

## 12. Software Requirements
- .NET 8 SDK, Visual Studio 2022 or VS Code, SQL Server LocalDB

## 13. Hardware Requirements
- Minimum 4 GB RAM, 2 GB free disk space, any modern CPU capable of running Visual Studio

## 14. Implementation
The system follows the MVC (Model-View-Controller) pattern. Models represent database entities via EF
Core Code First. Controllers contain business logic and enforce authorization. Views use Razor syntax
with Bootstrap 5 for styling. ViewModels decouple form input/validation from database entities.

## 15. Testing
See `Testing.md` for the full list of test cases covering registration, login, CRUD operations, search,
filters, reports, and authorization checks.

## 16. Future Scope
- PDF/Excel export of reports
- Recurring transactions
- Email/SMS budget alerts
- Multi-currency support

## 17. Advantages
- Simple, focused, and easy to understand/demonstrate
- Secure password storage and per-user data isolation
- Real-time calculated dashboard and reports (no hard-coded values)
- Clean, responsive UI

## 18. Limitations
- No bank account integration
- No multi-currency support
- No mobile app (web-only, though responsive)
- Single-server deployment model (no built-in horizontal scaling)

## 19. Conclusion
Personal Expense Tracker demonstrates a complete, secure, full-stack ASP.NET Core MVC application with
authentication, CRUD operations, relational database design, and data-driven reporting — suitable for
academic demonstration and viva examination, while also being genuinely useful as a simple personal
finance tool.
