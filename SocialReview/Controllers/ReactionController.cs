using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialReview.Models;
using SocialReview.Repositories.Interface;
using System.Threading.Tasks;

namespace SocialReview.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")] // URL sẽ là /api/Reaction
    [Authorize] // BẮT BUỘC: Người dùng phải đăng nhập mới được reaction
    public class ReactionController : ControllerBase
    {
        private readonly IReactionRepository _reactionRepo;
        private readonly UserManager<User> _userManager;

        public ReactionController(IReactionRepository reactionRepo, UserManager<User> userManager)
        {
            _reactionRepo = reactionRepo;
            _userManager = userManager;
        }


        //[HttpPost("toggle")]
        //public async Task<IActionResult> ToggleReaction([FromForm] int reviewId, [FromForm] string reactionType)
        //{
        //    // 1. Kiểm tra dữ liệu (Validation)
        //    if (reviewId <= 0 || string.IsNullOrWhiteSpace(reactionType))
        //    {
        //        return BadRequest(new { errors = "Dữ liệu không hợp lệ." });
        //    }

        //    // 2. Lấy UserId (An toàn)
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return Unauthorized(); // Lỗi 401
        //    }

        //    // 3. Gọi Repository để xử lý logic (Thêm/Xóa)
        //    // (Hàm này sẽ tự động kiểm tra xem user đã reaction chưa)
        //    bool userHasReacted = await _reactionRepo.ToggleReactionAsync(reviewId, user.Id, reactionType);

        //    // 4. Lấy số lượng (count) mới
        //    int newCount = await _reactionRepo.GetReactionCountAsync(reviewId, reactionType);

        //    // 5. Trả về kết quả (JSON)
        //    return Ok(new
        //    {
        //        newCount = newCount,
        //        userHasReacted = userHasReacted // Báo cho JS biết nên 'highlight' nút hay không
        //    });
        //}

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleReaction([FromForm] int reviewId, [FromForm] string reactionType)
        {
            try
            {
                if (reviewId <= 0 || string.IsNullOrWhiteSpace(reactionType))
                    return BadRequest(new { errors = "Dữ liệu không hợp lệ." });

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                bool userHasReacted = await _reactionRepo.ToggleReactionAsync(reviewId, user.Id, reactionType);
                int newCount = await _reactionRepo.GetReactionCountAsync(reviewId, reactionType);

                return Ok(new { newCount, userHasReacted });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi Reaction API: " + ex.Message);
                return StatusCode(500, new { errors = ex.Message });
            }
        }
    }
}