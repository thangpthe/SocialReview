
using SocialReview.Models;
namespace SocialReview.Repositories.Interface
    {
        public interface IAdminReviewRepository
        {
            // Review Management
            Task<IEnumerable<Review>> GetAllReviewsAsync();
            Task<IEnumerable<Review>> GetReviewsByStatusAsync(string status);
            Task<Review?> GetReviewByIdAsync(int id);
            Task DeleteReviewAsync(int id);
            Task UpdateReviewStatusAsync(int id, string status);

            // Comment Management
            Task<IEnumerable<Comment>> GetAllCommentsAsync();
            Task<Comment?> GetCommentByIdAsync(int id);
            Task DeleteCommentAsync(int id);

            // Statistics
            Task<int> GetTotalReviewsCountAsync();
            Task<int> GetTotalCommentsCountAsync();
            Task<int> GetPendingReviewsCountAsync();
        }
    }


