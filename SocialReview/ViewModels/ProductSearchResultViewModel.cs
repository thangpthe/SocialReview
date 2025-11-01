using SocialReview.Models;

namespace SocialReview.ViewModels
{
    public class ProductSearchResultViewModel
    {
        public Product Product { get; set; }
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
    }
}
