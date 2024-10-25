using ClipShare.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClipShare.ViewModels.Admin
{
    public class UserAddEdit_vm
    {
        public int Id { get; set; }
        [Display(Name = "Name (Username)")]
        [StringCustomValidation("Name", true, 3, 15, "^[a-zA-Z0-9_.-]*$", "Name must contain only a-z A-Z 0-9 characters")]
        public string Name { get; set; }
        [StringCustomValidation("Email", true, 0, 0, "^.+@[^\\.].*\\.[a-z]{2,}$", "Invalid email address")]
        public string Email { get; set; }
        [StringCustomValidation("Password", false, 6, 15, "^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,15}$", "Password must contain at least one letter, at least one number, and be between 6-15 characters in length with no special characters.")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Roles")]
        public List<string> UserRoles { get; set; } = new();
        public List<string> ApplicationRoles { get; set; } = new();
    }
}
