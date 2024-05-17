using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using test_indentity.Models;
using test_indentity.ViewModels;

namespace test_indentity.Controllers
{
    public class AccountController(SignInManager<AppUser> _signInManager, UserManager<AppUser> _userManager) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName!, model.UserPassword!, model.RememberMe, false);

                if(result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Неверная попытка входа");
                return View(model);
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if(ModelState.IsValid)
            {
                AppUser user = new()
                {
                    Name = model.Name!,
                    UserName = model.Email,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password!);

                if(result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(" ", error.Description);
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
    }
}
