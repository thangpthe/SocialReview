using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialReview.ViewModels
{
    public class ProductCRUDViewModel
    {
        public int ProductID { get; set; }

        // CompanyID sẽ được gán ở Controller, không cần [Required]

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        [Display(Name = "Danh mục")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200)]
        [Display(Name = "Tên Sản phẩm/Dịch vụ")]
        public string ProductName { get; set; }

        [Display(Name = "Mô tả")]
        public string? ProductDescription { get; set; }

        [StringLength(255)]
        [Url(ErrorMessage = "Link ảnh phải là một URL hợp lệ")]
        [Display(Name = "Link ảnh")]
        public string? ProductImage { get; set; }
        [Display(Name = "Giá")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá phải là một số dương.")]
        public int ProductPrice { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại.")]
        [StringLength(20)]
        [Display(Name = "Loại")]
        public string ProductType { get; set; }
        
    }
}
