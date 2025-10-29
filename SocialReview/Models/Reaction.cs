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
        [ForeignKey("User")]
        public int UserId { get; set; }
        [Required]
        public int ReviewID { get; set; }
        public string Type { get; set; }  // "Like", "Love", "Wow"
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public User User { get; set; }
        public Review Review { get; set; }

    }

}