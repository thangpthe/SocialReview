using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialReview.Models
{
    [Table("Companies")]

    public class Company
    {

        [Key]
        [Required]
        public int CompanyID { get; set; }
        public int UserID { get; set; }
        [Required(ErrorMessage = "Tên công ty là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên công ty tối đa 200 ký tự")]
        public string CompanyName { get; set; }

        [StringLength(255, ErrorMessage = "Đường dẫn ảnh không quá 255 ký tự")]
        public string Logo { get; set; }
        public string CompanyDescription { get; set; }
        // [Required(ErrorMessage = "")]
        [StringLength(100, ErrorMessage = "Tên lĩnh vực không quá 100 ký tự")]
        public string Industry { get; set; }

        [Url(ErrorMessage = "Tên website là 1 url")]
        [StringLength(200, ErrorMessage = "Độ dài url không quá 200 ký tự")]
        public string Website { get; set; }

        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Độ dài email không quá 100 ký tự")]
        public string ContactEmail { get; set; }
        [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không quá 20 chữ số")]
        public string Phone { get; set; }
        public User User { get; set; }
        public ICollection<Product>? Products { get; set; }


    }

}