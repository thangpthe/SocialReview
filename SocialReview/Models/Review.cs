using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialReview.Models
{
    [Table("Reviews")]

    public class Review
    {
        [Key]
        [Required]
        public int ReviewID { get; set; }
        [Required]
        public int ProductID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        public string Content { get; set; }
        [Required(ErrorMessage = "Đánh giá là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Số sao từ 1 đến 5")]
        public int Rating { get; set; }

        [StringLength(255, ErrorMessage = "Độ dài link ảnh không quá 255 ký tự")]
        public string Image { get; set; }
        [Required(ErrorMessage = "Loại sản phẩm là bắt buộc")]

        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Reaction>? Reactions { get; set; }

    }

}