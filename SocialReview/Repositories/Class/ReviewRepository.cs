using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.Repositories.Interface;

namespace SocialReview.Repositories.Class
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;
        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .AsNoTracking() // Tối ưu hóa (chỉ đọc)
                .Include(r => r.User) // Tải User (để lấy Tên, Avatar)
                .Include(r => r.Product) // Tải Product (để lấy Tên Sản phẩm)
                .Include(r => r.Comments) // Tải Comments (để đếm)
                .Include(r => r.Reactions) // Tải Reactions (để đếm)
                .FirstOrDefaultAsync(r => r.ReviewID == id);
        }

        public async Task<IEnumerable<Review>> GetLatestReviewsAsync(int count)
        {
            // Đây là một truy vấn "sâu" (deep)
            // Nó tải TẤT CẢ dữ liệu cần thiết cho _ReviewCardPartial
            return await _context.Reviews
                .AsNoTracking() // Tối ưu hóa (chỉ đọc)
                .OrderByDescending(r => r.CreatedAt) // Mới nhất
                .Take(count) // Lấy số lượng (ví dụ: 10)
                .Include(r => r.User) // Tải User (để lấy Tên, Avatar)
                .Include(r => r.Product) // Tải Product (để lấy Tên Sản phẩm)
                .Include(r => r.Comments) // Tải Comments (để đếm)
                .Include(r => r.Reactions) // Tải Reactions (để đếm)
                .ToListAsync();
        }

        public async Task UpdateAsync(Review reviewToUpdate)
        {
            _context.Reviews.Update(reviewToUpdate);

            // Lưu thay đổi vào DB
            await _context.SaveChangesAsync();
        }
    }
}
