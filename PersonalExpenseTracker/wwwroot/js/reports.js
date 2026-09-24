// Shared helpers for Chart.js report pages.
// Each report view supplies its own dataset via a JSON script block and calls these helpers.

function renderCategoryPieChart(canvasId, labels, data) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    new Chart(ctx, {
        type: "doughnut",
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: [
                    "#198754", "#0d6efd", "#fd7e14", "#dc3545", "#6f42c1",
                    "#20c997", "#ffc107", "#0dcaf0", "#6c757d", "#d63384"
                ]
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { position: "bottom" } }
        }
    });
}

function renderIncomeExpenseBarChart(canvasId, labels, incomeData, expenseData) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return;
    new Chart(ctx, {
        type: "bar",
        data: {
            labels: labels,
            datasets: [
                { label: "Income", data: incomeData, backgroundColor: "#198754" },
                { label: "Expense", data: expenseData, backgroundColor: "#dc3545" }
            ]
        },
        options: {
            responsive: true,
            plugins: { legend: { position: "bottom" } },
            scales: { y: { beginAtZero: true } }
        }
    });
}
