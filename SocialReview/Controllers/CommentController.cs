using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialReview.Models;
using SocialReview.Repositories.Interface;

namespace SocialReview.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ICommentRepository _commentRepo;

        public CommentController(ICommentRepository commentRepo, UserManager<User> userManager)
        {
            _commentRepo = commentRepo;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ReviewID, string Content)
        {
            // Kiểm tra validation thủ công (vì nó không phải ViewModel)
            if (string.IsNullOrWhiteSpace(Content))
            {
                ModelState.AddModelError("Content", "Vui lòng nhập nội dung bình luận.");
            }

            if (ModelState.IsValid)
            {
                // Lấy UserId (string) từ cookie
                var userIdString = _userManager.GetUserId(User);
                int.TryParse(userIdString, out int userIdInt);

                var comment = new Comment
                {
                    ReviewID = ReviewID,
                    CommentContent = Content,
                    UserId = userIdInt, // Lấy từ server (an toàn)
                    CreatedAt = DateTime.UtcNow
                };

                await _commentRepo.AddAsync(comment);
                var newCommentWithUser = await _commentRepo.GetByIdWithUserAsync(comment.CommentID);
                return PartialView("~/Views/Shared/_CommentCardPartial.cshtml", newCommentWithUser);
            }

            // Xử lý lỗi (trả về JSON cho AJAX)
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage);
            return BadRequest(new { errors = errors });
        }
        public IActionResult Index()
        {
            return View();
        }

        // ... (Constructor)

        // [HttpPost] Create (giữ nguyên)
        // ...

        // === THÊM 2 ACTION SAU ĐỂ SỬA ===

        [HttpGet]
        public async Task<IActionResult> GetComment(int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id); // Dùng hàm mới
            if (comment == null) return NotFound();

            // KIỂM TRA QUYỀN SỞ HỮU
            if (comment.UserId.ToString() != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return Ok(new { content = comment.CommentContent });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromForm] int CommentID, [FromForm] string Content)
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                return BadRequest(new { errors = new[] { "Nội dung không được rỗng." } });
            }

            var commentToUpdate = await _commentRepo.GetByIdAsync(CommentID);
            if (commentToUpdate == null) return NotFound();

            // KIỂM TRA QUYỀN SỞ HỮU
            if (commentToUpdate.UserId.ToString() != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            commentToUpdate.CommentContent = Content;
            await _commentRepo.UpdateAsync(commentToUpdate);

            return Ok(new { success = true, newContent = commentToUpdate.CommentContent });
        }

       

        // === THÊM ACTION MỚI ĐỂ XÓA ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            var commentToDelete = await _commentRepo.GetByIdAsync(id);
            if (commentToDelete == null)
            {
                return NotFound();
            }

            // KIỂM TRA QUYỀN SỞ HỮU
            if (commentToDelete.UserId.ToString() != _userManager.GetUserId(User))
            {
                return Forbid(); // Lỗi 403
            }

            await _commentRepo.DeleteAsync(commentToDelete);

            return Ok(new { success = true, message = "Đã xóa bình luận." });
        }
    }
}
