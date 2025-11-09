using SocialReview.Models;

public class CommentReaction
{
    public int CommentReactionID { get; set; }
    public int CommentID { get; set; }
    public int UserId { get; set; }

    // "Like", "Report", v.v.
    public string Type { get; set; }

    // Navigation properties
    public virtual Comment Comment { get; set; }
    public virtual User User { get; set; }
}