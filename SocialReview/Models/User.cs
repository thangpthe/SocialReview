using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialReview.Models
{
    [Table("Users")]
    public class User : IdentityUser<int>
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public string FullName { get; set; }
        [StringLength(255)]
        public string? UserAvatar { get; set; }
        [Required]
        [StringLength(20)]
        public string UserRole { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Company>? Companies { get; set; }
        //public ICollection<Review>? Reviews { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Reaction>? Reactions { get; set; }
        public ICollection<Report>? Reports { get; set; }

    }

}