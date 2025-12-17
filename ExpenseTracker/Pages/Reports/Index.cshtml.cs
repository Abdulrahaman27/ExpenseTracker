using ExpenseTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTracker.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly IReportService _reportService;

        [BindProperty(SupportsGet = true)]
        public int Year { get; set; } = DateTime.Now.Year;

        [BindProperty(SupportsGet = true)]
        public int Month { get; set; } = DateTime.Now.Month;

        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; } = DateTime.Now;

        public FinancialReport MonthlyReport { get; set; }
        public FinancialReport YearlyReport { get; set; }
        public FinancialReport CustomReport { get; set; }
        public List<MonthlySummary> MonthlySummaries { get; set; }
        public CategoryReport CategoryReport { get; set; }

        [TempData]
        public string Message { get; set; }

        [TempData]
        public string MessageType { get; set; } // "success", "error", "info"

        public IndexModel(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task OnGetAsync()
        {
            try
            {
                if (Year > 0)
                {
                    MonthlyReport = await _reportService.GenerateMonthlyReportAsync(Year, Month);
                    YearlyReport = await _reportService.GenerateYearlyReportAsync(Year);
                    MonthlySummaries = await _reportService.GetMonthlySummariesAsync(Year);
                }

                CustomReport = await _reportService.GenerateCustomReportAsync(StartDate, EndDate);
                CategoryReport = await _reportService.GetCategoryReportAsync(StartDate, EndDate);
            }
            catch (Exception ex)
            {
                Message = $"Error generating reports: {ex.Message}";
                MessageType = "error";
            }
        }

        public async Task<IActionResult> OnPostExportMonthlyAsync()
        {
            try
            {
                var report = await _reportService.GenerateMonthlyReportAsync(Year, Month);
                // Implement export logic here
                Message = "Monthly report exported successfully!";
                MessageType = "success";
            }
            catch (Exception ex)
            {
                Message = $"Error exporting report: {ex.Message}";
                MessageType = "error";
            }
            return RedirectToPage();
        }
    }
}