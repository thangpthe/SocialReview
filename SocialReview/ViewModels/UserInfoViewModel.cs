using SocialReview.Models;

namespace SocialReview.ViewModels
{
    public class UserInfoViewModel
    {
        public User UserProfile { get; set; }
        public IEnumerable<Review> UserReviews { get; set; }

    }

}
