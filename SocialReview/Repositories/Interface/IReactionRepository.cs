namespace SocialReview.Repositories.Interface
{
    public interface IReactionRepository
    {
        Task<bool> ToggleReactionAsync(int reviewId, int userId, string reactionType);

        /// <summary>
        /// Đếm số lượng reaction của một loại cụ thể cho một review
        /// </summary>
        /// <param name="reviewId">ID của bài Review</param>
        /// <param name="reactionType">Loại (vd: "Helpful")</param>
        /// <returns>Tổng số lượng</returns>
        Task<int> GetReactionCountAsync(int reviewId, string reactionType);
    }
}
