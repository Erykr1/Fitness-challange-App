using Microsoft.EntityFrameworkCore;

namespace DBContext.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Challenges> Challenge { get; set; }
        public DbSet<Sport> Sports { get; set; }
        public DbSet<User> Users { get; set; }
    }

    public class Challenges
    {
        public string Id { get; set; }
        public string? Goal { get; set; }
        public string? UserId { get; set; }
    }
    public class User
    {
        public string Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Biography { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }
    }
    public class Sport
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? JoinedUsers { get; set; }
        public string? IsActive { get; set; }        
        public string? Difficulty {get; set;}
    }
}