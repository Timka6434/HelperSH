using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using test_indentity.Models;
using test_indentity.ViewModels;

namespace test_indentity.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRolesViewModel = new List<UserListVM>();

            foreach (var user in users)
            {
                var thisViewModel = new UserListVM();
                thisViewModel.UserId = user.Id;
                thisViewModel.UserName = user.UserName;
                thisViewModel.Email = user.Email;
                thisViewModel.Roles = await GetUserRoles(user);
                userRolesViewModel.Add(thisViewModel);
            }

            return View(userRolesViewModel);
        }

        private async Task<List<string>> GetUserRoles(AppUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }
    }
}
