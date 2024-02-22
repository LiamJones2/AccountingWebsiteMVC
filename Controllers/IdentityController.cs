using AccountingWebsite.Data;
using AccountingWebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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
            return View();
        }
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Company");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostCompanyAndUser(Company companyModel, ApplicationUser userModel, string userPassword)
        {
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
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, CompanyId = model.CompanyId, LockoutEnabled = false, EmailConfirmed = true };
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
