using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialReview.Models
{
    [Table("Reports")]

    public class Report
    {
        [Key]
        [Required]

        public int ReportID { get; set; }
        [Required]
        public int ReporterID { get; set; }
        public string TargetType { get; set; }  // "Review" or "Comment"
        [Required]
        public int TargetID { get; set; }
        [Required(ErrorMessage = "Lý do không được để trống")]
        public string Reason { get; set; }
        public string? ReportDescription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public User Reporter { get; set; }
    }

}