using System.ComponentModel.DataAnnotations;

namespace ClipShare.ViewModels.Account
{
    public class Login_vm
    {
        private string _username;

        [Display(Name = "Username or Email")]
        [Required(ErrorMessage = "Username is required")]
        public string UserName {
            get => _username; 
            set => _username = !string.IsNullOrEmpty(value) ? value.ToLower() : null;
        }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
