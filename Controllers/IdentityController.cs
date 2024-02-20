using AccountingWebsite.Data;
using AccountingWebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccountingWebsite.Controllers
{
    public class IdentityController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Signup()
        {
            System.Diagnostics.Debug.WriteLine("Test");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostCompanyAndUser(Company companyModel, ApplicationUser userModel, string userPassword)
        {
            System.Diagnostics.Debug.WriteLine("Here");
            ModelState["userModel.Company"].ValidationState = ModelValidationState.Valid;
            if (await PostCompany(companyModel))
            {
                userModel.CompanyId = companyModel.CompanyId;
                if (await PostUser(userModel, userPassword))
                {
                    return RedirectToAction("Index", "Company");
                }
            }


            return View("Signup", ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> PostCompany(Company model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Adds the company to the database
                    _db.Companies.Add(model);
                    _db.SaveChanges();

                    System.Diagnostics.Debug.WriteLine("Company saved successfully.");
                    return true;
                }

                System.Diagnostics.Debug.WriteLine("Invalid model state for the company.");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving company: {ex.Message}");
                return false;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> PostUser(ApplicationUser model, string userPassword)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, CompanyId = model.CompanyId };
                var result = await _userManager.CreateAsync(user, userPassword);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return true;
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return false;
        }
    }
}
