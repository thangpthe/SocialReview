//using System.ComponentModel.DataAnnotations;

//namespace SocialReview.ViewModels
//{
//    public class EditReviewViewModel
//    {
//        public int ReviewID { get; set; }

//        [Required(ErrorMessage = "Tiêu đề không được để trống")]
//        [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
//        [Display(Name = "Tiêu đề")]
//        public string Title { get; set; }

//        [Required(ErrorMessage = "Nội dung không được để trống")]
//        [Display(Name = "Nội dung đánh giá")]
//        public string Content { get; set; }

//        [Required(ErrorMessage = "Vui lòng chọn điểm đánh giá")]
//        [Range(1, 5, ErrorMessage = "Điểm đánh giá từ 1 đến 5")]
//        [Display(Name = "Điểm đánh giá")]
//        public int Rating { get; set; }

//        [Display(Name = "Sản phẩm")]
//        public int ProductID { get; set; }

//        public string ProductName { get; set; }
//    }
//}
// Trong file: ViewModels/EditReviewViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace SocialReview.ViewModels
{
    public class EditReviewViewModel
    {
        // Hidden fields
        public int ReviewID { get; set; }
        public int ProductID { get; set; }

        // Hiển thị cho user, không cần edit
        [Display(Name = "Sản phẩm")]
        public string? ProductName { get; set; }

        // Các trường user có thể sửa
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(100, ErrorMessage = "Tiêu đề quá dài")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [Display(Name = "Nội dung đánh giá")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Bạn phải chọn điểm đánh giá")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5")]
        [Display(Name = "Điểm đánh giá")]
        public int Rating { get; set; }
    }
}