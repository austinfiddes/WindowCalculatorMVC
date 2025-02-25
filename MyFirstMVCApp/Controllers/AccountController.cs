using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyFirstMVCApp.Models;
using System.Threading.Tasks;

namespace MyFirstMVCApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home"); // Redirect logged-in users
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "User does not exist. Please register.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    Console.WriteLine($"Login successful for user: {model.Email}, RememberMe: {model.RememberMe}");

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = model.RememberMe ? DateTime.UtcNow.AddDays(7) : null
                    };

                    await _signInManager.SignInAsync(user, authProperties);

                    // ✅ Debug: Log authentication properties
                    Console.WriteLine($"Auth Properties: IsPersistent = {authProperties.IsPersistent}, ExpiresUtc = {authProperties.ExpiresUtc}");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Console.WriteLine("❌ Login failed!");
                    ModelState.AddModelError("", "Invalid email or password. Please try again.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("Register attempt with email: " + model.Email);

                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError("", "Email and Password are required.");
                    return View(model);
                }

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "An account with this email already exists.");
                    return View(model);
                }

                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    Console.WriteLine($"✅ User {user.Email} created successfully!");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Console.WriteLine($"❌ Registration failed for {user.Email}");
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                Console.WriteLine("❌ ModelState is invalid.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            // Ensure all authentication cookies are removed
            Response.Cookies.Delete(".AspNetCore.Identity.Application");

            return RedirectToAction("Login", "Account");
        }
    }
}