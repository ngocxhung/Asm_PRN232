using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement_WebApp.Pages.Admin
{
    public class CategoriesModel : PageModel
    {
        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return RedirectToPage("/Login");
            return Page();
        }
    }
} 