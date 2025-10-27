using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialReview.Models
{
    [Table("Users")]
    public class User
    {

        [Key]
        [Required]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public string FullName { get; set; }
        [EmailAddress(ErrorMessage = "Sai định dạng email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [StringLength(100, ErrorMessage = "Email có độ dài tối đa 100 ký tự")]

        public string PasswordHash { get; set; }
        // [Required(ErrorMessage = "")]
        public string? Avatar { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Company>? Companies { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Reaction>? Reactions { get; set; }
        public ICollection<Report>? Reports { get; set; }

    }

}