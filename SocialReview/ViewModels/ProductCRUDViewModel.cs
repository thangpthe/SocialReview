using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SocialReview.ViewModels
{
    // ViewModel này được dùng bởi Modal AJAX
    public class ProductCRUDViewModel
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        public string ProductName { get; set; }

        public string? ProductDescription { get; set; }

        [Display(Name = "Ảnh hiện tại (URL)")]
        public string? ProductImage { get; set; } // Giữ lại để hiển thị ảnh cũ

        [Display(Name = "Tải lên ảnh mới")]
        public IFormFile? ImageFile { get; set; } // THÊM MỚI: Dùng để upload file

        [Range(0,int.MaxValue, ErrorMessage = "Giá phải là số không âm.")]
        public int ProductPrice { get; set; }

        public string? ProductType { get; set; }
    }
}