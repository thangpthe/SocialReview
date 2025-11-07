using SocialReview.Models;

namespace SocialReview.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalCompanies { get; set; }
        public int TotalProducts { get; set; }
        public int TotalReviews { get; set; }
        public List<Review> RecentReviews { get; set; }
    }
}
