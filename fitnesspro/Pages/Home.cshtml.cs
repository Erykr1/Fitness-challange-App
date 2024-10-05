using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DBContext.Data;

namespace MyApp.Namespace
{
    public class HomeModel : PageModel
    {
         private readonly ApplicationDbContext _context;

            public HomeModel(ApplicationDbContext context)
            {
                _context = context;
            }

            public List<Sport> Sports { get; set; }
            public Dictionary<string, List<string>> SportUserEmails { get; set; } = new Dictionary<string, List<string>>();

            public async Task OnGetAsync(string difficultyFilter = null)
            {
                if (!string.IsNullOrEmpty(difficultyFilter) && difficultyFilter != "all")
                {
                    IQueryable<Sport> query = _context.Sports;
                    string difficulty = difficultyFilter;
                    query = query.Where(s => s.Difficulty == difficulty);
                    Sports = await query.ToListAsync();
                }
                else{
                    Sports = await _context.Sports.ToListAsync();
                }
                

                foreach (var sport in Sports)
                {
                    if (!string.IsNullOrEmpty(sport.JoinedUsers))
                    {
                        var userIds = sport.JoinedUsers.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(id => id.Trim())
                                                    .ToList();

                        var userEmails = await _context.Users
                                                    .Where(user => userIds.Contains(user.Id))
                                                    .Select(user => user.Email)
                                                    .ToListAsync();
                        
                        SportUserEmails[sport.Id] = userEmails;
                    }
                }
            }
            public async Task<IActionResult> OnPostJoinSportAsync(string sportId)
            {
                var sport = await _context.Sports.FindAsync(sportId);
                if (sport == null)
                {
                    return NotFound();
                }

                string userId = Request.Cookies["UserID"];
                if (string.IsNullOrEmpty(userId))
                {
                    return Page();
                }

                if (!string.IsNullOrEmpty(sport.JoinedUsers) && sport.JoinedUsers.Split(',').Contains(userId))
                {
                    TempData["AlertMessage"] = "Bu etkinliğe daha önce katılmışsınız.";
                    TempData["AlertType"] = "warning";
                    return RedirectToPage();
                }
                else
                {
                    if (string.IsNullOrEmpty(sport.JoinedUsers))
                    {
                        sport.JoinedUsers = userId;
                    }
                    else
                    {
                        sport.JoinedUsers += "," + userId;
                    }

                    await _context.SaveChangesAsync();
                    TempData["AlertMessage"] = "Etkinliğe başarıyla katıldınız.";
                    TempData["AlertType"] = "success";
                    return RedirectToPage();
                }
            }
    }
}
