using AccountingWebsite.Data;
using AccountingWebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;

namespace AccountingWebsite.Controllers
{
    public class CompanyController : Controller
    {
        private CompanyData _company { get; set; }
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        private int _revenueTypeId;
        private int _expenseTypeId;

        public CompanyController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) {
            _company = new CompanyData {
                quarter = new QuarterInfo()
            };
            _db = db;
            _userManager = userManager;

            _revenueTypeId = GetTransactionTypeId("Revenue");
            _expenseTypeId = GetTransactionTypeId("Expense");
        }

        private int GetTransactionTypeId(string typeName)
        {
            return _db.TransactionTypes
                .Where(tt => tt.TypeName == typeName)
                .Select(tt => tt.TransactionTypeId)
                .FirstOrDefault();
        }

        public async Task<IActionResult> Index(DateTime? start, DateTime? end)
        {
            if (!start.HasValue || !end.HasValue)
            {
                // If start or end is not provided in the URL, use the default quarter
                GetCurrentQuarter();
            }
            else
            {
                _company.quarter = new QuarterInfo
                {
                    StartDate = start.Value,
                    EndDate = end.Value
                };
            }

            await updateCompanyData();
            return View(_company);
        }

        public async Task<CompanyData> updateCompanyData()
        {
            var user = await _userManager.GetUserAsync(User);

            var company = _db.Companies.FirstOrDefault(c => c.CompanyId == user.CompanyId);

            if (company != null)
            {
                var quarterlyTransactions = _db.Transactions
                    .Where(t => t.CompanyId == company.CompanyId &&
                        t.TransactionDate >= _company.quarter.StartDate &&
                        t.TransactionDate <= _company.quarter.EndDate)
                    .ToList();


                var overallRevenue = quarterlyTransactions
                    .Where(t => t.TransactionType == _revenueTypeId)
                    .Sum(t => t.Amount);

                var overallExpense = quarterlyTransactions
                    .Where(t => t.TransactionType == _expenseTypeId)
                    .Sum(t => t.Amount);

                _company = new CompanyData
                {
                    Company = company,
                    QuarterlyTransactions = quarterlyTransactions,
                    OverallRevenue = overallRevenue,
                    OverallExpenses = overallExpense,
                    quarter = _company.quarter
                };
            }
            return _company;
        }

        public void GetCurrentQuarter()
        {
            DateTime currentDate = DateTime.Now;
            int quarter = (currentDate.Month - 1) / 3 + 1;

            DateTime startDate = new DateTime(currentDate.Year, (quarter - 1) * 3 + 1, 1);
            DateTime endDate = startDate.AddMonths(3).AddDays(-1);

            _company.quarter = new QuarterInfo
            {
                StartDate = startDate,
                EndDate = endDate
            };
        }


        [HttpPost]
        public async Task<IActionResult> UpdateQuarter(DateTime? quarter)
        {
            System.Diagnostics.Debug.WriteLine("Changing Quarter");

            // Update quarter dates
            _company.quarter.StartDate = quarter;
            _company.quarter.EndDate = quarter?.AddMonths(3).AddDays(-1);

            // Redirect to Index with updated quarter parameters
            return RedirectToAction(nameof(Index), new { start = _company.quarter.StartDate, end = _company.quarter.EndDate });
        }
    }
}