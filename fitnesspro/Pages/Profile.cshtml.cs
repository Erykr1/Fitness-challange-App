using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBContext.Data;
using Microsoft.EntityFrameworkCore;
namespace MyApp.Namespace
{
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public ProfileModel(ApplicationDbContext context)
        {
            _context = context;
        }

    [BindProperty]
    public string SelectedChallengeId { get; set; }  // Challenge ID'si için property

    [BindProperty]
    public string Goal { get; set; }  // Goal input field'ı

    // public List<Challenges> UserChallenges { get; set; }

        public string UploadedImagePath { get; set; }
        [BindProperty]
        public IFormFile ImageFile { get; set; }
        [BindProperty]
        public string Biography { get; set; }
        [BindProperty]
        public string NewPassword { get; set; }
        [BindProperty]
        public string Address { get; set; }
        [BindProperty]
        public string UserId { get; set; } 
        public List<Sport> UserChallenges { get; set; }
        public async Task OnGetAsync()
        {
            string userId = Request.Cookies["UserID"]; 

            UserChallenges = await _context.Sports
                                        .Where(c => c.JoinedUsers.Contains(userId)) // Kullanıcıya ait zorlukları filtrele
                                        .ToListAsync();

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                Biography = user.Biography;
                Address = user.Address;
                UploadedImagePath = user.Image;  // Resim yolu veya adı
            }
        }
        public async Task<IActionResult> OnPostSetGoalAsync()
        {
            if (!int.TryParse(Request.Form["SelectedChallengeId"], out int selectedChallengeId))
            {
                // Hata mesajı göster
                ModelState.AddModelError("", "Invalid challenge ID.");
                return Page();
            }

            var challenge = await _context.Challenge.FindAsync(selectedChallengeId);
            if (challenge == null)
            {
                // Hata mesajı göster
                ModelState.AddModelError("", "Challenge not found.");
                return Page();
            }

            // Challenge'i güncelle
            challenge.Goal = Goal;
            await _context.SaveChangesAsync();
            return RedirectToPage(); // veya başka bir işlem
        }



        public async Task<IActionResult> OnPostAsync()
        {
            var userId = Request.Cookies["UserID"];
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Message"] = "UserID is missing in the cookie.";
                TempData["MessageType"] = "danger";
                return RedirectToPage();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                TempData["Message"] = "User not found.";
                TempData["MessageType"] = "danger"; 
                return RedirectToPage();
            }

            if (ImageFile != null && !string.IsNullOrEmpty(ImageFile.FileName))
            {
                var filePath = Path.Combine("wwwroot/images", ImageFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                user.Image = ImageFile.FileName;
            }
            else
            {
                // Dosya yüklenmediğinde yapılacak işlemler
            }

            user.Biography = Biography;

            if(!string.IsNullOrEmpty(NewPassword)){
                user.Password = NewPassword;
            }
            
            user.Address = Address;

            _context.Attach(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            TempData["Message"] = "Profile updated successfully.";
            TempData["MessageType"] = "success"; 
            return RedirectToPage();
        }
    }
}
