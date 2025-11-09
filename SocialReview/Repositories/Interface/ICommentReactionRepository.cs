namespace SocialReview.Repositories.Interface
{
    public interface ICommentReactionRepository
    {
        Task<bool> ToggleReactionAsync(int commentId, int userId, string reactionType);
        Task<int> GetReactionCountAsync(int commentId, string reactionType);
    }
}
