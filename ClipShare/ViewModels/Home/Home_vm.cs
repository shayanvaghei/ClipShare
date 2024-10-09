using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ClipShare.ViewModels.Home
{
    public class Home_vm
    {
        public string Page { get; set; }
        public IEnumerable<SelectListItem> CategoryDropdown { get; set; }
    }
}
