using AccountingWebsite.Data;
using AccountingWebsite.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccountingWebsite.Controllers
{
    public class CompanyController : Controller
    {
        private Company company { get; set; }
        private readonly ApplicationDbContext _db;
        public CompanyController(ApplicationDbContext db) {
            _db = db;
        }

        public IActionResult Index()
        {

            return View(company);
        }
    }
}
