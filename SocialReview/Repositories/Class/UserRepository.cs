using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.Repositories.Interface;

namespace SocialReview.Repositories.Class
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

       
        public UserRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        //public async Task<bool> ExistsByEmail(string email)
        //{
        //   return await _dbSet.AnyAsync(u => u.UserEmail == email);
        //}

        //public async Task<User?> GetUserByUsername(string username)
        //{
        //    return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        //}

        //public async Task<User?> GetUserById(int id)
        //{
        //    return await _dbSet.FirstOrDefaultAsync(u => u.UserID == id);
        //}


        //public async Task<User?> Authenticate(string username, string password)
        //{

        //    var user = await GetUserByUsername(username);
        //    if (user == null)
        //    {
        //        return null;
        //    }

        //    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        //    return isPasswordValid ? user : null;
        //}

        //public async Task saveUserAsync(User user)
        //{
        //    await _context.Users.AddAsync(user);
        //    await _context.SaveChangesAsync();
        //}
        public async Task<IEnumerable<User>> GetAllUser()
        {
            return await _context.Users.ToListAsync();
        }
            

        //public async Task<User> GetUserById(int id)
        //{
        //    return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        //}
    }
}
