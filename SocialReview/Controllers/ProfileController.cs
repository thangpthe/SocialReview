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
    
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(UserManager<User> userManager, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        private int GetCurrentUserId()
        {
            // ClaimTypes.NameIdentifier chính là User.Id
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdString); // Chuyển string sang int
        }

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
        //public async Task<IActionResult> EditReview(int id)
        //{
        //    var currentUserId = GetCurrentUserId();
        //    var review = await _context.Reviews.FindAsync(id);

        //    // 6. KIỂM TRA BẢO MẬT: 
        //    // - Review có tồn tại không?
        //    // - User này có PHẢI là chủ của review này không?
        //    if (review == null || review.UserId != currentUserId)
        //    {
        //        return Forbid(); // Trả về lỗi 403 Forbidden (Không có quyền)
        //    }

        //    // (Bạn nên tạo một ReviewViewModel thay vì dùng Model trực tiếp)
        //    return View(review); // Gửi data đến Views/Profile/EditReview.cshtml
        //}

        //
        // POST: /Profile/EditReview/5
        //
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditReview(int id, Review reviewModel)
        //{
        //    if (id != reviewModel.ReviewID)
        //    {
        //        return BadRequest();
        //    }

        //    var currentUserId = GetCurrentUserId();

        //    // 7. KIỂM TRA BẢO MẬT LẦN NỮA:
        //    // Lấy review gốc từ CSDL để kiểm tra sở hữu
        //    var originalReview = await _context.Reviews
        //                                .AsNoTracking() // Không theo dõi
        //                                .FirstOrDefaultAsync(r => r.ReviewID == id);

        //    if (originalReview == null || originalReview.UserId != currentUserId)
        //    {
        //        return Forbid(); // Không có quyền sửa
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            // Đảm bảo UserID và các trường nhạy cảm không bị thay đổi
        //            reviewModel.UserId = currentUserId;
        //            reviewModel.CreatedAt = originalReview.CreatedAt; // Giữ ngày tạo gốc

        //            _context.Update(reviewModel);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            throw; // Xử lý lỗi
        //        }
        //        return RedirectToAction(nameof(Index)); // Quay về trang profile
        //    }
        //    return View(reviewModel);
        //}

        //public async Task<IActionResult> EditReview(int id)
        //{
        //    var currentUserId = GetCurrentUserId();
        //    var review = await _context.Reviews
        //        .Include(r => r.Product)
        //        .FirstOrDefaultAsync(r => r.ReviewID == id);

        //    // Kiểm tra quyền sở hữu
        //    if (review == null || review.UserId != currentUserId)
        //    {
        //        TempData["ErrorMessage"] = "Không tìm thấy review hoặc bạn không có quyền chỉnh sửa.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var viewModel = new EditReviewViewModel
        //    {
        //        ReviewID = review.ReviewID,
        //        Title = review.Title,
        //        Content = review.Content,
        //        Rating = review.Rating,
        //        ProductID = review.ProductID,
        //        ProductName = review.Product?.ProductName
        //    };

        //    return View(viewModel);
        //}

        public async Task<IActionResult> EditReview(int id)
        {
            var currentUserId = GetCurrentUserId();
            var review = await _context.Reviews
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.ReviewID == id);

            // Kiểm tra quyền sở hữu
            if (review == null || review.UserId != currentUserId)
            {
                TempData["ErrorMessage"] = "Không tìm thấy review hoặc bạn không có quyền chỉnh sửa.";
                return RedirectToAction(nameof(Index));
            }

            // Tạo ViewModel để gửi sang View
            var viewModel = new EditReviewViewModel
            {
                ReviewID = review.ReviewID,
                Title = review.Title,
                Content = review.Content,
                Rating = review.Rating,
                ProductID = review.ProductID,
                ProductName = review.Product?.ProductName ?? "Không rõ"
            };

            return View(viewModel);
        }

        // POST: /Profile/EditReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview(EditReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Load lại ProductName khi có lỗi validation
                var product = await _context.Products.FindAsync(model.ProductID);
                model.ProductName = product?.ProductName ?? "Không rõ";
                return View(model);
            }

            var currentUserId = GetCurrentUserId();
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ReviewID == model.ReviewID);

            // Kiểm tra quyền sở hữu
            if (review == null || review.UserId != currentUserId)
            {
                TempData["ErrorMessage"] = "Không tìm thấy review hoặc bạn không có quyền chỉnh sửa.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Cập nhật thông tin review
                review.Title = model.Title;
                review.Content = model.Content;
                review.Rating = model.Rating;

                _context.Reviews.Update(review);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật review thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Load lại ProductName khi có lỗi
                var product = await _context.Products.FindAsync(model.ProductID);
                model.ProductName = product?.ProductName ?? "Không rõ";

                ModelState.AddModelError(string.Empty, "Lỗi khi lưu: " + ex.Message);
                return View(model);
            }
        }

        // ===== DELETE REVIEW =====

        // POST: /Profile/DeleteReview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var currentUserId = GetCurrentUserId();
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ReviewID == id);

            // Kiểm tra quyền sở hữu
            if (review == null || review.UserId != currentUserId)
            {
                TempData["ErrorMessage"] = "Không tìm thấy review hoặc bạn không có quyền xóa.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa review thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> EditProfile()
        {
            var currentUserId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng." });
            }

            var viewModel = new EditProfileViewModel
            {
                FullName = user.FullName,
                UserAvatar = user.UserAvatar,
                Email = user.Email
            };

            // THAY ĐỔI 1: Trả về PartialView thay vì View
            return PartialView("_EditProfilePartial", viewModel);
        }

        // POST: /Profile/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // THAY ĐỔI 2: Validation thất bại, trả về PartialView với lỗi
                Response.StatusCode = 400; // Báo cho AJAX biết đây là lỗi
                return PartialView("_EditProfilePartial", model);
            }

            var currentUserId = GetCurrentUserId();
            var user = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (user == null)
            {
                Response.StatusCode = 404; // Not Found
                return Json(new { success = false, message = "Không tìm thấy người dùng." });
            }

            user.FullName = model.FullName;
            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                // 1. Tạo đường dẫn thư mục (ví dụ: wwwroot/images/avatars)
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatars");
                // 2. Tạo thư mục nếu chưa tồn tại
                Directory.CreateDirectory(uploadDir);

                // 3. Tạo tên file độc nhất (dùng UserId và GUID để tránh trùng lặp)
                string fileExtension = Path.GetExtension(model.AvatarFile.FileName);
                string uniqueFileName = $"{user.Id}_{Guid.NewGuid()}{fileExtension}";

                // 4. Đường dẫn lưu file vật lý
                string filePath = Path.Combine(uploadDir, uniqueFileName);

                // 5. Lưu file vào server
                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.AvatarFile.CopyToAsync(fileStream);
                    }

                    // 6. Cập nhật URL mới cho user
                    // (Đây là đường dẫn web mà trình duyệt có thể truy cập)
                    user.UserAvatar = $"/images/avatars/{uniqueFileName}";
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu không lưu được file
                    ModelState.AddModelError("AvatarFile", "Lỗi khi tải lên tệp: " + ex.Message);
                    Response.StatusCode = 400;
                    return PartialView("_EditProfilePartial", model);
                }

                // (Tùy chọn: Bạn có thể thêm logic ở đây để xóa file avatar cũ
                // nếu nó không phải là ảnh mặc định)
            }
            // Nếu model.AvatarFile == null (người dùng không tải ảnh mới),
            // chúng ta không làm gì cả, user.UserAvatar sẽ giữ nguyên giá trị cũ.

            // === KẾT THÚC THAY ĐỔI LOGIC AVATAR ===

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {

                return Json(new
                {
                    success = true,
                    newFullName = user.FullName,
                    newAvatar = user.UserAvatar, // Trả về URL mới (hoặc cũ)
                    newInitial = string.IsNullOrEmpty(user.FullName) ? "" : user.FullName.Substring(0, 1)
                });
            }
            else
            {
                // THAY ĐỔI 4: Lỗi từ Identity, trả về PartialView
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                Response.StatusCode = 400; // Bad Request
                return PartialView("_EditProfilePartial", model);
            }
        }
    }
}

