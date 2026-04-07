using SocialReview.Models;
using System.Collections.Generic;

namespace SocialReview.ViewModels
{
    public class ProductSearchViewModel
    {
        //public IEnumerable<Product> Products { get; set; } = new List<Product>();

        //// 2. Các tùy chọn cho bộ lọc
        //public List<string> TypeOptions { get; set; } = new List<string> { "Sản phẩm", "Dịch vụ" };
        //public List<int> RatingOptions { get; set; } = new List<int> { 5, 4, 3, 2, 1 };

        //// 3. Các giá trị filter người dùng đã chọn (để giữ lại trên form)
        //public string? CurrentType { get; set; }
        //public int? CurrentRating { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public string? CurrentType { get; set; }
        public int? CurrentRating { get; set; }

        // Thêm 2 thuộc tính này
        public List<string> TypeOptions { get; set; } = new List<string>();
        public List<int> RatingOptions { get; set; } = new List<int> { 1, 2, 3, 4, 5 };

    }
}

