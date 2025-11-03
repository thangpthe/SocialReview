using SocialReview.Models;
using System;
using System.Collections.Generic;

namespace SocialReview.ViewModels
{
    // Model này chứa dữ liệu cho trang Profile/Index.cshtml
    public class ProfileViewModel
    {
        // Thông tin user
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string? UserAvatar { get; set; }
        public DateTime CreatedAt { get; set; }

        // Danh sách review của user này
        public List<Review> MyReviews { get; set; }
    }

    // Model này dùng cho form SỬA HỒ SƠ (sửa logo/avatar)
    public class EditProfileViewModel
    {
        public string FullName { get; set; }
        public string? UserAvatar { get; set; } // Link ảnh mới
    }
}
