using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.Repositories.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialReview.Repositories.Class
{
    /// <summary>
    /// Triển khai (Implement) logic cho IReactionRepository
    /// </summary>
    public class ReactionRepository : IReactionRepository
    {
        private readonly ApplicationDbContext _context;

        public ReactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Đếm số lượng reaction
        /// </summary>
        public async Task<int> GetReactionCountAsync(int reviewId, string reactionType)
        {
            // Truy vấn CSDL và đếm các reaction khớp
            return await _context.Reactions
                .CountAsync(r => r.ReviewID == reviewId && r.Type == reactionType);
        }

        /// <summary>
        /// Logic "Toggle" (Bật/Tắt) Reaction
        /// </summary>
        public async Task<bool> ToggleReactionAsync(int reviewId, int userId, string reactionType)
        {
            // 1. Tìm xem user này đã reaction (cùng loại) vào review này chưa
            var existingReaction = await _context.Reactions
                .FirstOrDefaultAsync(r => r.ReviewID == reviewId &&
                                          r.UserId == userId &&
                                          r.Type == reactionType);

            // 2. Nếu ĐÃ CÓ (tức là người dùng nhấn "Unlike")
            if (existingReaction != null)
            {
                _context.Reactions.Remove(existingReaction); // Xóa reaction
                await _context.SaveChangesAsync();
                return false; // Trả về: User "không" còn reaction nữa
            }
            // 3. Nếu CHƯA CÓ (tức là người dùng nhấn "Like")
            else
            {
                // --- SỬA LỖI 500 (THÊM BƯỚC KIỂM TRA) ---
                // 3a. Kiểm tra xem ReviewID có hợp lệ không
                var reviewExists = await _context.Reviews.AnyAsync(r => r.ReviewID == reviewId);

                // (Giả sử UserID luôn hợp lệ vì Controller đã kiểm tra)

                if (!reviewExists)
                {
                    // Nếu Review không tồn tại, không làm gì cả
                    return false;
                }
                // --- KẾT THÚC SỬA LỖI ---

                // 3b. Chỉ tạo Reaction nếu Review tồn tại
                var newReaction = new Reaction
                {
                    ReviewID = reviewId,
                    UserId = userId,
                    Type = reactionType,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Reactions.AddAsync(newReaction); // Thêm reaction mới
                await _context.SaveChangesAsync();
                return true; // Trả về: User "đã" reaction
            }
        }
    }
}

