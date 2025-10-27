using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialReview.Models
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [Required]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Tên loại là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên loaij tối đa 100 ký tự")]
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public ICollection<Product> Products { get; set; }




    }

}