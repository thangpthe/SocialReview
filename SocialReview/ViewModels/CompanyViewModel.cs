using SocialReview.Models;

namespace SocialReview.ViewModels
{
    public class CompanyViewModel
    {
        public Company CompanyProfile { get; set; }
        public IEnumerable<Product> ProductList { get; set; }
    }
}
