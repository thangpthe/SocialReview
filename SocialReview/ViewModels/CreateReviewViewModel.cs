using System.ComponentModel.DataAnnotations;

namespace SocialReview.ViewModels
{
    public class CreateReviewViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [MaxLength(200,ErrorMessage = "Tiêu đề không quá 200 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng viết nội dung")]
        [MaxLength(2000,ErrorMessage = "Nội dung không quá 2000 ký tự")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn số sao")]
        [Range(1, 5, ErrorMessage = "Rating phải từ 1 đến 5")]
        public int Rating { get; set; }
    }
}
