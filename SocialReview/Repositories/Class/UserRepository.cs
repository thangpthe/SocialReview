using Microsoft.EntityFrameworkCore;
using SocialReview.Models;
using SocialReview.Repositories.Interface;

namespace SocialReview.Repositories.Class
{
    public class UserRepository : IUserRepository,Repository<User>
    {
        private readonly ApplicationDbContext _context;
        public UserRepository (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            if ((await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == email)) == null)
            {
                return false;
            }
            return true;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == email);
        }


        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            //get user by email
            var user = await GetUserByEmailAsync(email);
            if (user == null)
            {
                return false;
            }
            //xác thực mật khẩu
            return user.VerifyPassword(password);
        }

        public async Task saveUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
