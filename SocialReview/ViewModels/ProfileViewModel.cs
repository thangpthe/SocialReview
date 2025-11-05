using SocialReview.Models;
using System.ComponentModel.DataAnnotations;

namespace SocialReview.ViewModels
{
    // ViewModel cho trang Index
    public class ProfileViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Review> MyReviews { get; set; }
    }

    // ViewModel cho từng review item
   
}