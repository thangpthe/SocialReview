using SocialReview.Models;

namespace SocialReview.Repositories.Interface
{
    public interface IReviewRepository
    {
        Task AddAsync(Review review);
    }
}
