using System.ComponentModel.DataAnnotations;

namespace SocialReview.ViewModels
{
    // ViewModel cho trang Index
    public class ProfileViewModel
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ReviewItemViewModel> MyReviews { get; set; } = new List<ReviewItemViewModel>();
    }

    // ViewModel cho từng review item
    public class ReviewItemViewModel
    {
        public int ReviewID { get; set; }
        public string Title { get; set; }
        public int Rating { get; set; }
        public int ProductID { get; set; }
        public ProductBasicInfo Product { get; set; }
    }

    public class ProductBasicInfo
    {
        public string ProductName { get; set; }
    }

    // ViewModel cho chỉnh sửa thông tin cá nhân
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Avatar hiện tại")]
        public string CurrentAvatar { get; set; }

        [Display(Name = "Tải lên avatar mới")]
        public IFormFile AvatarFile { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới (để trống nếu không đổi)")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
    }

    // ViewModel cho chỉnh sửa review
    public class EditReviewViewModel
    {
        public int ReviewID { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được quá 200 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [Display(Name = "Nội dung đánh giá")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn điểm đánh giá")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá từ 1 đến 5")]
        [Display(Name = "Điểm đánh giá")]
        public int Rating { get; set; }

        [Display(Name = "Sản phẩm")]
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        [Display(Name = "Hình ảnh hiện tại")]
        public List<string> CurrentImages { get; set; } = new List<string>();

        [Display(Name = "Thêm hình ảnh mới")]
        public List<IFormFile> NewImages { get; set; }

        [Display(Name = "Xóa hình ảnh cũ")]
        public List<int> ImagesToDelete { get; set; } = new List<int>();
    }
}