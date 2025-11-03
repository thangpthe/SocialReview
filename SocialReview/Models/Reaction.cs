using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialReview.Models
{
    [Table("Reactions")]

    public class Reaction
    {

        [Key]
        [Required]
        public int ReactionID { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [Required]
        [ForeignKey("ReviewID")]
        public int ReviewID { get; set; }
        public virtual Review Review { get; set; }

        public string Type { get; set; }  // "Like", "Love", "Wow"
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation

    }

}