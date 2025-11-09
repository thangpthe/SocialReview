using SocialReview.Models;

namespace SocialReview.Repositories.Interface
{
    public interface IReviewRepository
    {
        Task AddAsync(Review review);

        Task<IEnumerable<Review>> GetLatestReviewsAsync(int count);
        Task<Review?> GetByIdAsync(int id);
        Task UpdateAsync(Review reviewToUpdate);
    }
}
