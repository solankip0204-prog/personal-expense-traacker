namespace PersonalExpenseTracker.ViewModels
{
    public class DashboardViewModel
    {
        public string UserName { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance => TotalIncome - TotalExpenses;
        public decimal CurrentMonthExpenses { get; set; }
        public int TransactionCount { get; set; }
        public List<RecentTransactionViewModel> RecentTransactions { get; set; } = new();
    }

    public class RecentTransactionViewModel
    {
        public string Type { get; set; } // "Income" or "Expense"
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
