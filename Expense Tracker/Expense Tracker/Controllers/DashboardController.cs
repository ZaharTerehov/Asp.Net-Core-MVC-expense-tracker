using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Expense_Tracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            var startDate = DateTime.Today.AddDays(-6);
            var endDate = DateTime.Today;

            //Last 7 Days
            var selectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= startDate && y.Date <= endDate)
                .ToListAsync();

            //Total Income
            var totalIncome = selectedTransactions
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = totalIncome.ToString("C0");

			//Total Expense
			var totalExpense = selectedTransactions
				.Where(i => i.Category.Type == "Income")
				.Sum(j => j.Amount);
			ViewBag.TotalIncome = totalExpense.ToString("C0");

            //Balance
            var balance = totalIncome - totalExpense;
            var culture = CultureInfo.CreateSpecificCulture("en-US");

            culture.NumberFormat.CurrencyNegativePattern = 1;

            ViewBag.Balance = String.Format(culture, "{0:C0}", balance);

            //Doughnut Chart - Expense By Category
            ViewBag.DoughnutChartData = selectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                }).ToList();

			return View();
        }
    }
}
