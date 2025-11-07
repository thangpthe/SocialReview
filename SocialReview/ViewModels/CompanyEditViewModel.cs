using System.ComponentModel.DataAnnotations;

namespace SocialReview.ViewModels
{
    public class CompanyEditViewModel
    {
        

        [Required(ErrorMessage = "Tên công ty là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên công ty tối đa 200 ký tự")]
        public string CompanyName { get; set; }

        [StringLength(255, ErrorMessage = "Đường dẫn ảnh không quá 255 ký tự")]
        [Display(Name = "Link Logo")]
        public string? Logo { get; set; }
        [Display(Name = "Tải lên logo mới")]
        public IFormFile? LogoFile { get; set; }

        [Display(Name = "Mô tả Doanh nghiệp")]
        public string? CompanyDescription { get; set; }

        [StringLength(100, ErrorMessage = "Tên lĩnh vực không quá 100 ký tự")]
        [Display(Name = "Ngành (Industry)")]
        public string? Industry { get; set; }

        [Url(ErrorMessage = "Phải là một URL hợp lệ (ví dụ: https://...)")]
        [StringLength(200, ErrorMessage = "Độ dài URL không quá 200 ký tự")]
        public string? Website { get; set; }

        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Độ dài email không quá 100 ký tự")]
        [Display(Name = "Email Liên hệ")]
        public string? ContactEmail { get; set; }

        [Phone(ErrorMessage = "Định dạng số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không quá 20 chữ số")]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }
    }
}
