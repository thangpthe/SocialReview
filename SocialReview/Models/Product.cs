using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialReview.Models
{
    [Table("Products")]

    public class Product
    {

        [Key]
        [Required]
        public int ProductID { get; set; }
        public int CompanyID { get; set; }
        public int CategoryID { get; set; }
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên công ty tối đa 200 ký tự")]
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        // [Required(ErrorMessage = "")]
        [StringLength(255, ErrorMessage = "Link ảnh có độ dài không quá 255 ký tự")]
        public string ProductImage { get; set; }

        [StringLength(200, ErrorMessage = "Độ dài url không quá 200 ký tự")]
        public int ProductPrice { get; set; }
        [Required(ErrorMessage = "Loại sản phẩm là bắt buộc")]
        public string ProductType { get; set; }

        public DateTime? CreatedAt { get; set; }
        public Company Company { get; set; }
        public Category Category{ get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

    }

}