using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SocialReview.Services
{
    public class SlugService : ISlugService
    {
        public string GenerateSlug(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                // Trả về một slug ngẫu nhiên nếu text rỗng
                return $"item-{Guid.NewGuid().ToString().Substring(0, 6)}";
            }

            // 1. Chuyển sang chữ thường và bỏ dấu
            string slug = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in slug)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            slug = sb.ToString().Normalize(NormalizationForm.FormC);

            // 2. Xử lý chữ 'đ'
            slug = slug.Replace('đ', 'd');

            // 3. Xóa các ký tự đặc biệt không mong muốn
            // Chỉ giữ lại chữ cái (a-z), số (0-9), khoảng trắng, và gạch nối
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // 4. Thay thế khoảng trắng bằng gạch nối
            slug = Regex.Replace(slug.Trim(), @"\s+", "-"); // Thay một hoặc nhiều khoảng trắng
            slug = Regex.Replace(slug, @"-+", "-"); // Thay nhiều gạch nối (nếu có) bằng 1

            // (Tùy chọn) 5. Cắt bớt độ dài
            if (slug.Length > 100)
            {
                slug = slug.Substring(0, 100);
            }

            // 6. Xử lý trường hợp slug rỗng sau khi lọc (ví dụ: "!!!")
            if (string.IsNullOrWhiteSpace(slug))
            {
                slug = $"item-{Guid.NewGuid().ToString().Substring(0, 6)}";
            }

            return slug;
        }
    }
}