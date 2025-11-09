using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.Repositories.Interface;

public class CommentReactionRepository : ICommentReactionRepository
{
    private readonly ApplicationDbContext _context;

    public CommentReactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ToggleReactionAsync(int commentId, int userId, string reactionType)
    {
        var existingReaction = await _context.CommentReactions
            .FirstOrDefaultAsync(r => r.CommentID == commentId &&
                                      r.UserId == userId &&
                                      r.Type == reactionType);

        if (existingReaction != null)
        {
            // Đã reaction -> Xóa
            _context.CommentReactions.Remove(existingReaction);
            await _context.SaveChangesAsync();
            return false; // User không còn reaction
        }
        else
        {
            // Chưa reaction -> Thêm
            var newReaction = new CommentReaction
            {
                CommentID = commentId,
                UserId = userId,
                Type = reactionType
            };
            await _context.CommentReactions.AddAsync(newReaction);
            await _context.SaveChangesAsync();
            return true; // User đã reaction
        }
    }

    public async Task<int> GetReactionCountAsync(int commentId, string reactionType)
    {
        return await _context.CommentReactions
            .CountAsync(r => r.CommentID == commentId && r.Type == reactionType);
    }
}