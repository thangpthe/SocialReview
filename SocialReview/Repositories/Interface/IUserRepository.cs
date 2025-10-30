using SocialReview.Models;

namespace SocialReview.Repositories.Interface
{
    public interface IUserRepository
    {
        //Task<User> GetUserById(int id);
        Task<IEnumerable<User>> GetAllUser();
    }
}
