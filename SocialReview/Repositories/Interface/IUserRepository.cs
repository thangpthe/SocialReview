using SocialReview.Models;

namespace SocialReview.Repositories.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<User?> GetUserByEmail(string email);
        Task<bool>AuthenticateAsync(string email, string password);
        Task SaveUserAsync(User user);
        Task<User> GetUserById(int id);
    }
}
