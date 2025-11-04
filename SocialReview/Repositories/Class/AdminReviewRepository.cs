using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.Repositories.Interface;

namespace SocialReview.Repositories.Class
{
    public class AdminReviewRepository : IAdminReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== REVIEW MANAGEMENT =====
        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .Include(r => r.Comments)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByStatusAsync(string status)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .Include(r => r.Comments)
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .Include(r => r.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(r => r.ReviewID == id);
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateReviewStatusAsync(int id, string status)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                review.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        // ===== COMMENT MANAGEMENT =====
        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Review)
                    .ThenInclude(r => r.Product)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Review)
                    .ThenInclude(r => r.Product)
                .FirstOrDefaultAsync(c => c.CommentID == id);
        }

        public async Task DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        // ===== STATISTICS =====
        public async Task<int> GetTotalReviewsCountAsync()
        {
            return await _context.Reviews.CountAsync();
        }

        public async Task<int> GetTotalCommentsCountAsync()
        {
            return await _context.Comments.CountAsync();
        }

        public async Task<int> GetPendingReviewsCountAsync()
        {
            return await _context.Reviews
                .Where(r => r.Status == "Pending")
                .CountAsync();
        }
    }
}