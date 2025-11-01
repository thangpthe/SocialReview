using SocialReview.Models;
using System.Collections.Generic;

namespace SocialReview.ViewModels
{
    public class ProductDetailViewModel
    {
        // 1. Thông tin sản phẩm chính (tên, mô tả, ảnh...)
        public Product Product { get; set; }

        // 2. Danh sách các review đã được viết cho sản phẩm này
        public IEnumerable<Review> Reviews { get; set; }

        // 3. Form để người dùng nhập review MỚI
        public CreateReviewViewModel NewReviewForm { get; set; }

        // --- Các thông tin thống kê thêm ---
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
    }
}