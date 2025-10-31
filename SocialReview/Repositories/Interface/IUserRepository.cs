using SocialReview.Models;

namespace SocialReview.Repositories.Interface
{
    public interface IUserRepository
    {
        //Task<User> GetUserById(int id);
        Task<User?> GetUserByUsername(string username);
        
        Task<IEnumerable<User>> GetAllUser();
    }
}
