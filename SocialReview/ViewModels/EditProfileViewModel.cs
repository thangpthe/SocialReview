using System.ComponentModel.DataAnnotations;

namespace SocialReview.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [Display(Name = "Họ và tên")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Display(Name = "URL Ảnh đại diện (Avatar)")]
        [Url(ErrorMessage = "URL ảnh đại diện không hợp lệ")]
        public string? UserAvatar { get; set; }

        // Hiển thị email nhưng không cho sửa
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}