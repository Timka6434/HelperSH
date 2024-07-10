using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using test_indentity.Models;
using test_indentity.ViewModels;

namespace test_indentity.Controllers
{
    [Authorize(Roles = "Admin, Engineer")]
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
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserListVM>();

            foreach (var user in users)
            {
                var thisViewModel = new UserListVM
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    Email = user.Email,
                    Roles = await GetUserRoles(user),
                    IsActive = user.IsActive
                };
                userRolesViewModel.Add(thisViewModel);
            }

            ViewBag.AllRoles = await _roleManager.Roles.ToListAsync();

            return View(userRolesViewModel);
        }

        private async Task<List<string>> GetUserRoles(AppUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.UserName,
                    Name = model.Name,
                    Email = model.EmailAddress,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.UserPassword);
                if (result.Succeeded)
                {
                    // Проверяем, что выбранная роль существует
                    if (!await _roleManager.RoleExistsAsync(model.SelectedRole))
                    {
                        ModelState.AddModelError("", "Указанная роль не существует.");
                        var roles = await _roleManager.Roles.ToListAsync();
                        ViewBag.Roles = roles;
                        return View(model);
                    }

                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            var allRoles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = allRoles;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserVM
            {
                UserId = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Email = user.Email,
                SelectedRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };

            ViewBag.Roles = await _roleManager.Roles.ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.Name = model.Name;
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                    if (!removePasswordResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Не удалось удалить старый пароль");
                        return View(model);
                    }

                    var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
                    if (!addPasswordResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Не удалось установить новый пароль");
                        return View(model);
                    }
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var result = await _userManager.RemoveFromRolesAsync(user, userRoles);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Не удалось удалить старые роли пользователя");
                    return View(model);
                }

                result = await _userManager.AddToRoleAsync(user, model.SelectedRole);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Не удалось добавить новую роль пользователю");
                    return View(model);
                }

                result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string userId, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = isActive;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Не удалось изменить статус пользователя");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = false; // Вместо удаления деактивируем пользователя
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Не удалось удалить пользователя");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Получаем текущего пользователя
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

            // Запрещаем инженерам назначать роли Admin
            if (currentUserRoles.Contains("Engineer") && role == "Admin")
            {
                ModelState.AddModelError("", "Инженеры не могут назначать роли Admin.");
                return RedirectToAction("Index");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Не удалось удалить старые роли пользователя");
                return RedirectToAction("Index");
            }

            result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Не удалось добавить новую роль пользователю");
                return RedirectToAction("Index");
            }

            TempData["Message"] = "Роль пользователя успешно обновлена";
            return RedirectToAction("Index");
        }
    }
}
