using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using DBContext.Data;
using Microsoft.EntityFrameworkCore;

namespace fitnesspro.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context; // DbContext ekleniyor

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public string LoginEmail { get; set; }
        [BindProperty]
        public string LoginPassword { get; set; }
        [BindProperty]
        public string RegisterEmail { get; set; }
        [BindProperty]
        public string RegisterPassword { get; set; }
        [BindProperty]
        public string Action { get; set; }

        public IActionResult OnGet()
        {
            var userId = Request.Cookies["UserID"];
            if (!string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Home");
            }
            return Page();
        }
        

        public IActionResult OnPost()
        {
            if (Action == "login")
            {
                return HandleLogin();
            }
            else if (Action == "register")
            {
                return HandleRegister();
            }
            return Page();
        }

        private IActionResult HandleLogin()
        {
            var user = _context.Users
                        .FirstOrDefault(u => u.Email == LoginEmail && u.Password == LoginPassword);
            if (user != null)
            {
                Response.Cookies.Append("UserID", user.Id, new CookieOptions 
                { 
                    HttpOnly = true, 
                    Secure = false
                });
                return RedirectToPage("Home");
            }
            else
            {
                // Giriş başarısız
                ModelState.AddModelError(string.Empty, "Giriş bilgileri hatalı.");
                return Page();
            }
        }

        private IActionResult HandleRegister()
        {
            if (!string.IsNullOrEmpty(RegisterEmail) && !string.IsNullOrEmpty(RegisterPassword))
            {
                var newUser = new User { Id = Guid.NewGuid().ToString(), Email = RegisterEmail, Password = RegisterPassword };
                _context.Users.Add(newUser);
                _context.SaveChanges(); // Veritabanına kayıt
                return RedirectToPage("Home"); // Kayıt başarılı sayfasına yönlendir
            }
            else
            {
                // Kayıt başarısız
                ModelState.AddModelError(string.Empty, "Kayıt işlemi başarısız.");
                return Page();
            }
        }
    }
}