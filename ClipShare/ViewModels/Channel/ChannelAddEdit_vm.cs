using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClipShare.ViewModels.Channel
{
    public class ChannelAddEdit_vm
    {
        [Required]
        [Display(Name = "Channel name")]
        [RegularExpression("^[a-zA-Z]{3,15}", ErrorMessage = "Name must be between 3 and 15 characters long and can only contain letters (A-Z, a-z)")]
        public string Name { get; set; }
        [Required(ErrorMessage = "About field is required")]
        [StringLength(200, MinimumLength = 20, ErrorMessage = "About must be at least {2}, and maximum {1} characters")]
        [Display(Name = "About your channel")]
        public string About { get; set; }
        public List<ModelError_vm> Errors { get; set; } = new List<ModelError_vm>();
        public int SubscribersCount { get; set; }
    }
}
