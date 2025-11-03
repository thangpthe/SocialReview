using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // 1. Bắt buộc để bảo mật
using Microsoft.AspNetCore.Identity;
using SocialReview.Models;
using SocialReview.ViewModels;
using SocialReview.Data; // 2. Thêm DbContext của bạn (hoặc Repository)
using System.Threading.Tasks;
using System.Security.Claims; // 3. Dùng để lấy ID user
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SocialReview.Controllers
{
    // 4. BẮT BUỘC [Authorize] (Chỉ user đã đăng nhập mới vào được)
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context; // Hoặc IReviewRepository...

        public ProfileController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // --- Hàm helper để lấy ID của user hiện tại ---
        private int GetCurrentUserId()
        {
            // ClaimTypes.NameIdentifier chính là User.Id
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdString); // Chuyển string sang int
        }

        //
        // GET: /Profile/Index (hoặc /Profile)
        //
        public async Task<IActionResult> Index()
        {
            var currentUserId = GetCurrentUserId();

            // 1. Lấy thông tin user
            var user = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            // 2. Lấy danh sách review CHỈ CỦA USER NÀY
            var myReviews = await _context.Reviews
                .Where(r => r.UserId == currentUserId) // 5. KIỂM TRA SỞ HỮU
                .Include(r => r.Product) // Lấy kèm thông tin Product
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            // 3. Tạo ViewModel để gửi sang View
            var viewModel = new ProfileViewModel
            {
                UserId = user.Id.ToString(),
                FullName = user.FullName,
                Email = user.Email,
                Username = user.UserName,
                UserAvatar = user.UserAvatar,
                CreatedAt = user.CreatedAt,
                MyReviews = myReviews
            };

            return View(viewModel); // Gửi data đến Views/Profile/Index.cshtml
        }

        //
        // GET: /Profile/EditReview/5
        //
        public async Task<IActionResult> EditReview(int id)
        {
            var currentUserId = GetCurrentUserId();
            var review = await _context.Reviews.FindAsync(id);

            // 6. KIỂM TRA BẢO MẬT: 
            // - Review có tồn tại không?
            // - User này có PHẢI là chủ của review này không?
            if (review == null || review.UserId!= currentUserId)
            {
                return Forbid(); // Trả về lỗi 403 Forbidden (Không có quyền)
            }

            // (Bạn nên tạo một ReviewViewModel thay vì dùng Model trực tiếp)
            return View(review); // Gửi data đến Views/Profile/EditReview.cshtml
        }

        //
        // POST: /Profile/EditReview/5
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview(int id, Review reviewModel)
        {
            if (id != reviewModel.ReviewID)
            {
                return BadRequest();
            }

            var currentUserId = GetCurrentUserId();

            // 7. KIỂM TRA BẢO MẬT LẦN NỮA:
            // Lấy review gốc từ CSDL để kiểm tra sở hữu
            var originalReview = await _context.Reviews
                                        .AsNoTracking() // Không theo dõi
                                        .FirstOrDefaultAsync(r => r.ReviewID == id);

            if (originalReview == null || originalReview.UserId != currentUserId)
            {
                return Forbid(); // Không có quyền sửa
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Đảm bảo UserID và các trường nhạy cảm không bị thay đổi
                    reviewModel.UserId = currentUserId;
                    reviewModel.CreatedAt = originalReview.CreatedAt; // Giữ ngày tạo gốc

                    _context.Update(reviewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw; // Xử lý lỗi
                }
                return RedirectToAction(nameof(Index)); // Quay về trang profile
            }
            return View(reviewModel);
        }

        //
        // POST: /Profile/DeleteReview/5
        //
        
    } 

}