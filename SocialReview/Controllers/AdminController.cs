using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SocialReview.Models;
using System.Linq;
using System.Threading.Tasks;
using SocialReview;

namespace ReviewSocialNetwork.Controllers
{
    // BƯỚC QUAN TRỌNG: Bảo vệ toàn bộ Controller này
    // Chỉ những ai đã đăng nhập VÀ có Role = "Admin" mới được vào
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Biến này dùng để truy cập CSDL (ví dụ: ApplicationDbContext)
        // Bạn cần inject DbContext của mình vào đây
        private readonly ApplicationDbContext _context;

        // Constructor để inject DbContext
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CHỨC NĂNG 1: Hiển thị trang Dashboard
        // GET: /Admin/Index hoặc /Admin
        [HttpGet]
        public IActionResult Index()
        {
            // Lấy 15 bài viết đang chờ duyệt để hiển thị ra bảng
            // (Giả sử bạn có model 'Review' với trạng thái 'Pending')
            var pendingReviews = _context.Reviews
                                        .Where(r => r.Status == "Pending")
                                        .OrderByDescending(r => r.CreatedAt)
                                        .Take(15)
                                        .ToList();

            // Gửi danh sách này sang View (trang admin.html/Index.cshtml)
            return View(pendingReviews);
        }

        // CHỨC NĂNG 2: Xử lý khi Admin nhấn nút "Duyệt"
        // API này được gọi từ JavaScript (fetch) trong file admin.html
        // POST: /Admin/ApproveReview/5
        [HttpPost]
        public async Task<IActionResult> ApproveReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy bài viết." });
            }

            // Cập nhật trạng thái
            review.Status = "Approved";
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            // Trả về JSON cho JavaScript biết là đã thành công
            return Ok(new { success = true, message = "Duyệt bài thành công." });
        }

        // CHỨC NĂNG 3: Xử lý khi Admin nhấn nút "Từ chối"
        // POST: /Admin/RejectReview/5
        [HttpPost]
        public async Task<IActionResult> RejectReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy bài viết." });
            }

            // Cập nhật trạng thái
            review.Status = "Rejected";
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Từ chối bài thành công." });
        }

        // Bạn có thể thêm các chức năng khác ở đây (quản lý User, xem báo cáo...)
    }
}
