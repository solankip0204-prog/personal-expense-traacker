# Testing Document: Personal Expense Tracker

> Note: These test cases are designed to be executed manually (or automated later) once the project is
> built and running locally, since build/runtime verification could not be performed in the environment
> that generated this project (no .NET SDK / no network access). "Actual Result" and "Status" columns
> are left blank for you to fill in during your own test run.

| Test Case ID | Test Case                          | Input                                                   | Expected Result                                              | Actual Result | Status |
|---------------|-------------------------------------|----------------------------------------------------------|----------------------------------------------------------------|----------------|--------|
| TC01 | User registration | Valid Full Name, Email, Password, Confirm Password, Phone | Account created, redirected to Login with success message | | |
| TC02 | Duplicate email registration | Email already used by an existing user | Error: "This email is already registered..." | | |
| TC03 | User login | Valid registered email + correct password | Redirected to Dashboard | | |
| TC04 | Invalid login | Wrong password or unregistered email | Error: "Invalid email or password." | | |
| TC05 | Admin login | admin@gmail.com / Admin@123 | Redirected to Admin Dashboard | | |
| TC06 | Add expense | Amount=250, Category=Food, valid date, payment method | Expense saved, appears in Expense list | | |
| TC07 | Edit expense | Change amount of an existing expense | Updated amount reflected in list and dashboard totals | | |
| TC08 | Delete expense | Delete an existing expense | Expense removed from list; dashboard totals recalculated | | |
| TC09 | Add income | Amount=5000, Source, Category, valid date | Income saved, appears in Income list | | |
| TC10 | Edit income | Change amount of an existing income record | Updated amount reflected in list and dashboard totals | | |
| TC11 | Delete income | Delete an existing income record | Income removed from list; dashboard totals recalculated | | |
| TC12 | Add category | New category name + type (Income/Expense) | Category appears in Category list | | |
| TC13 | Edit category | Rename a user-created category | Updated name shown everywhere it's used | | |
| TC14 | Delete category (unused) | Delete a category with no transactions | Category deleted successfully | | |
| TC15 | Delete category (in use) | Try deleting a category used by a transaction | Blocked with message: "cannot be deleted because it is used..." | | |
| TC16 | Add budget | Category=Food, Amount=5000, current month/year | Budget appears on Budget page with 0% used initially | | |
| TC17 | Edit budget | Change budget amount | Progress bar percentage recalculates | | |
| TC18 | Delete budget | Delete an existing budget | Budget removed from Budget page | | |
| TC19 | Expense search | Search by partial description text | Only matching expenses shown | | |
| TC20 | Expense filter | Filter by category + date range + amount range | Only expenses matching all filters shown | | |
| TC21 | Monthly report | Select a month/year with known transactions | Totals match manually calculated sum | | |
| TC22 | Dashboard calculations | Add known income/expense amounts | Dashboard totals equal sum of added amounts | | |
| TC23 | Unauthorized page access | Visit /Dashboard or /Admin while logged out | Redirected to Login page | | |
| TC24 | Logout | Click Logout while logged in | Session/cookie cleared, redirected to Login | | |
| TC25 | Password validation | Enter a 3-character password on Register | Validation error: minimum 6 characters | | |
| TC26 | Form validation | Submit Add Expense form with empty required fields | Validation errors shown, form not submitted | | |

## How to Run These Tests
1. Follow the README to restore, migrate and run the project.
2. Register a new test account (or use a seeded sample user).
3. Work through each test case above in order, recording the actual result and marking Pass/Fail.
4. For admin-related test cases, log out of the user account first, then log in via `/Account/AdminLogin`.
