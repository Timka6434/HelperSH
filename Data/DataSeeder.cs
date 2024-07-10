using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using test_indentity.Models;

namespace test_indentity.Data
{
    public class DataSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public DataSeeder(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedData()
        {
            // Создаем роли, если их еще нет
            string[] roleNames = ["Admin", "Engineer", "Director", "Teacher", "Head Teacher"];
            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            // Создаем администратора, если его еще нет
            var adminLogin = "AutoCreated";
            var adminName = "Admin";
            var adminEmail = "admin@example.com";
            var adminPassword = "Admin@123";

            if (await _userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new AppUser
                {
                    Name = adminLogin,
                    UserName = adminName,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
