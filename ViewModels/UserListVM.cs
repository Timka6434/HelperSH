using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace test_indentity.ViewModels
{
    public class UserListVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
        public IList<IdentityRole> AllRoles { get; set; } // Все доступные роли
        public IList<string> SelectedRoles { get; set; } // Роли, выбранные для пользователя
    }
}
