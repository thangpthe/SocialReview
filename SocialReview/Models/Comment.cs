using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialReview.Models
{
    [Table("Comments")]

    public class Comment
    {
        [Key]
        [Required]
        public int CommentID { get; set; }
        [Required]
        public int ReviewID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public Review Review { get; set; }
        public User User { get; set; }



    }

}