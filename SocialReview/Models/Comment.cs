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
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        public string CommentContent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public Review Review { get; set; }



    }

}