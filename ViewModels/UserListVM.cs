﻿namespace test_indentity.ViewModels
{
    public class UserListVM
    {
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public IList<string>? Roles { get; set; }
        public bool IsActive { get; set; }
    }
}
