namespace SocialReview.Models
{
    public class ReviewImage
    {
        public int ImageID { get; set; }
        public int ReviewID { get; set; }
        public string ImageUrl { get; set; }

        // Navigation
        public Review Review { get; set; }
    }
}