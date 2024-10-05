using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGetLogout()
        {
            Response.Cookies.Delete("UserID");  // Cookie'yi sil

            return RedirectToPage("/Index");  // Ana sayfaya yönlendir
        }
    }
}
