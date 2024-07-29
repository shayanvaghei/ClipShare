using System.ComponentModel.DataAnnotations;

namespace ClipShare.ViewModels.Account
{
    public class Register_vm
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Display(Name = "Name (Username)")]
        [Required(ErrorMessage = "Name (Username) is required")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Name must be at least {2}, and maximum {1} characters")]
        [RegularExpression("^[a-zA-Z0-9_.-]*$", ErrorMessage = "Name must contain only a-z A-Z 0-9 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression("^(?=.*[0-9]+.*)(?=.*[a-zA-Z]+.*)[0-9a-zA-Z]{6,15}$", ErrorMessage = "Password must contain at least one letter, at least one number, and be between 6-15 characters in length with no special characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
