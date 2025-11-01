using SocialReview.Models;

namespace SocialReview.Repositories.Interface
{
    public interface ICategoryRepository
    {
        Task<Category> GetAllCategory();
    }
}
