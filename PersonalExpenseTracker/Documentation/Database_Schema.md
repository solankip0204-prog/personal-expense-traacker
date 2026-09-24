# Database Schema: PersonalExpenseTrackerDb

## Tables

### Users
| Column        | Type            | Constraints                  |
|---------------|-----------------|-------------------------------|
| UserId        | int             | PK, Identity                  |
| FullName      | nvarchar(100)   | Required                      |
| Email         | nvarchar(150)   | Required, Unique               |
| PasswordHash  | nvarchar(max)   | Required (PBKDF2 hash)        |
| PhoneNumber   | nvarchar(15)    | Required                      |
| CreatedDate   | datetime2       | Default: current date/time    |

### Admins
| Column        | Type            | Constraints                  |
|---------------|-----------------|-------------------------------|
| AdminId       | int             | PK, Identity                  |
| FullName      | nvarchar(100)   | Required                      |
| Email         | nvarchar(150)   | Required, Unique               |
| PasswordHash  | nvarchar(max)   | Required                      |
| CreatedDate   | datetime2       | Default: current date/time    |

### Categories
| Column        | Type            | Constraints                              |
|---------------|-----------------|--------------------------------------------|
| CategoryId    | int             | PK, Identity                              |
| UserId        | int, nullable   | FK → Users.UserId (NULL = system default) |
| CategoryName  | nvarchar(100)   | Required                                  |
| CategoryType  | int (enum)      | Required (0 = Income, 1 = Expense)        |

### Expenses
| Column         | Type              | Constraints                     |
|----------------|-------------------|-----------------------------------|
| ExpenseId      | int               | PK, Identity                     |
| UserId         | int               | FK → Users.UserId, Required      |
| CategoryId     | int               | FK → Categories.CategoryId       |
| Amount         | decimal(18,2)     | Required, > 0                    |
| Description    | nvarchar(250)     | Required                         |
| ExpenseDate    | date              | Required                         |
| PaymentMethod  | int (enum)        | Required                         |
| Notes          | nvarchar(500)     | Optional                         |
| CreatedDate    | datetime2         | Default: current date/time       |

### Incomes
| Column        | Type              | Constraints                     |
|---------------|-------------------|-----------------------------------|
| IncomeId      | int               | PK, Identity                     |
| UserId        | int               | FK → Users.UserId, Required      |
| CategoryId    | int               | FK → Categories.CategoryId       |
| Amount        | decimal(18,2)     | Required, > 0                    |
| Source        | nvarchar(150)     | Required                         |
| IncomeDate    | date              | Required                         |
| Description   | nvarchar(250)     | Optional                         |
| Notes         | nvarchar(500)     | Optional                         |
| CreatedDate   | datetime2         | Default: current date/time       |

### Budgets
| Column        | Type              | Constraints                     |
|---------------|-------------------|-----------------------------------|
| BudgetId      | int               | PK, Identity                     |
| UserId        | int               | FK → Users.UserId, Required      |
| CategoryId    | int               | FK → Categories.CategoryId       |
| Amount        | decimal(18,2)     | Required, > 0                    |
| Month         | int               | Required, 1-12                   |
| Year          | int               | Required                         |
| CreatedDate   | datetime2         | Default: current date/time       |

## Relationships

- **User → Expense**: One-to-Many (cascade delete)
- **User → Income**: One-to-Many (cascade delete)
- **User → Budget**: One-to-Many (cascade delete)
- **User → Category**: One-to-Many (cascade delete, for user-created categories)
- **Category → Expense**: One-to-Many (restrict delete — cannot delete a category in use)
- **Category → Income**: One-to-Many (restrict delete)
- **Category → Budget**: One-to-Many (restrict delete)
- **Admin**: Independent table used only for administrative login/management; not linked by foreign key
  to Users (an admin manages users/categories/transactions through application logic, not a DB relation).

## Text ER Diagram

```
User
 |
 |----< Expense >---- Category
 |
 |----< Income  >---- Category
 |
 |----< Budget  >---- Category
 |
 |----< Category (user-created)

Admin  (independent; manages Users, Categories, Transactions via application logic)
```
