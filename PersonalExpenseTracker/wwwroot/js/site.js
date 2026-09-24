// Site-wide small enhancements
document.addEventListener("DOMContentLoaded", function () {
    // Auto-dismiss alerts after 5 seconds
    document.querySelectorAll(".alert").forEach(function (alertEl) {
        setTimeout(function () {
            var bsAlert = bootstrap.Alert.getOrCreateInstance(alertEl);
            if (bsAlert) bsAlert.close();
        }, 5000);
    });

    // Enable Bootstrap tooltips if present
    var tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltipTriggerList.forEach(function (el) {
        new bootstrap.Tooltip(el);
    });
});
