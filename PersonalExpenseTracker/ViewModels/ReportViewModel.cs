namespace PersonalExpenseTracker.ViewModels
{
    public class MonthlySummaryViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Balance => TotalIncome - TotalExpenses;
        public int NumberOfExpenses { get; set; }
        public decimal HighestExpense { get; set; }
        public decimal AverageExpense { get; set; }
    }

    public class CategoryReportItem
    {
        public string CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class ReportViewModel
    {
        public MonthlySummaryViewModel Summary { get; set; }
        public List<CategoryReportItem> ExpenseByCategory { get; set; } = new();
        public List<string> ChartMonthLabels { get; set; } = new();
        public List<decimal> ChartMonthlyIncome { get; set; } = new();
        public List<decimal> ChartMonthlyExpense { get; set; } = new();
    }
}
