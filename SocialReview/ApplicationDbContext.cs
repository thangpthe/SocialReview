using Microsoft.EntityFrameworkCore;

namespace SocialReview
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        
        
        }
        //DbSet<>
    }
}
