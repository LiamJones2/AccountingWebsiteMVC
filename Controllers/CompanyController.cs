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
            // Assuming you have a way to identify the currently signed-in user
            //string userId = "user_id"; // Replace with your actual user ID logic

            //// Retrieve the company associated with the signed-in user
            //Company company = _db.Companies.FirstOrDefault(c => c.UserId == userId);

            //if (company == null)
            //{
            //    // Handle the case where the user is not associated with any company
            //    return NotFound("Company not found for the current user.");
            //}

            // Pass the company data to the view
            return View(company);
        }
    }
}
