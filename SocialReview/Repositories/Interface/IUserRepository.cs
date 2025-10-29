using SocialReview.Models;

namespace SocialReview.Repositories.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        //Task<bool> ExistsByEmail(string email);
        //Task<User?> GetUserByUsername(string username);
        //Task<User?> Authenticate(string username, string password);
        ////public Task SaveUserAsync(User user);
        //Task<User> GetUserById(int id);
    }
}
