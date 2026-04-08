using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Repositories.Interface;

namespace SocialReview.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReviewController : Controller
    {
        private readonly IAdminReviewRepository _adminReviewRepo;
        private readonly ApplicationDbContext _context;

        public ReviewController(IAdminReviewRepository adminReviewRepo,ApplicationDbContext context)
        {
            _adminReviewRepo = adminReviewRepo;
            _context = context;
        }

        // ===== REVIEWS =====
        public async Task<IActionResult> Reviews(string? status)
        {
            IEnumerable<SocialReview.Models.Review> reviews;

            if (!string.IsNullOrEmpty(status))
            {
                reviews = await _adminReviewRepo.GetReviewsByStatusAsync(status);
            }
            else
            {
                reviews = await _adminReviewRepo.GetAllReviewsAsync();
            }

            ViewBag.CurrentStatus = status;
            ViewBag.TotalReviews = await _adminReviewRepo.GetTotalReviewsCountAsync();
            ViewBag.PendingReviews = await _adminReviewRepo.GetPendingReviewsCountAsync();

            return View(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await _adminReviewRepo.DeleteReviewAsync(id);
                TempData["SuccessMessage"] = "Xóa review thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Reviews));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateReviewStatus(int id, string status)
        {
            try
            {
                await _adminReviewRepo.UpdateReviewStatusAsync(id, status);
                TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Reviews));
        }

        // ===== COMMENTS =====
        public async Task<IActionResult> Comments()
        {
            var comments = await _adminReviewRepo.GetAllCommentsAsync();
            ViewBag.TotalComments = await _adminReviewRepo.GetTotalCommentsCountAsync();
            return View(comments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                await _adminReviewRepo.DeleteCommentAsync(id);
                TempData["SuccessMessage"] = "Xóa comment thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Comments));
        }

        // ===== AJAX ENDPOINTS =====
        [HttpPost]
        public async Task<IActionResult> QuickDeleteReview(int id)
        {
            try
            {
                await _adminReviewRepo.DeleteReviewAsync(id);
                return Json(new { success = true, message = "Xóa thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> QuickDeleteComment(int id)
        {
            try
            {
                await _adminReviewRepo.DeleteCommentAsync(id);
                return Json(new { success = true, message = "Xóa thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .Include(r => r.Comments)
                    .ThenInclude(c => c.User) // Lấy thông tin người bình luận
                .FirstOrDefaultAsync(r => r.ReviewID == id);

            if (review == null) return NotFound();
            return View(review);
        }
    }
}