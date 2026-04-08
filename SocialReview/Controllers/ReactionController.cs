using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialReview.Models;
using SocialReview.Repositories.Interface;
using System.Threading.Tasks;

namespace SocialReview.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Bắt buộc đăng nhập
    public class ReactionController : ControllerBase
    {
        private readonly IReactionRepository _reactionRepo;
        private readonly UserManager<User> _userManager;

        public ReactionController(IReactionRepository reactionRepo, UserManager<User> userManager)
        {
            _reactionRepo = reactionRepo;
            _userManager = userManager;
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleReaction([FromForm] int reviewId, [FromForm] string reactionType)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized(); // 401

            bool userHasReacted = await _reactionRepo.ToggleReactionAsync(reviewId, user.Id, reactionType);
            int newCount = await _reactionRepo.GetReactionCountAsync(reviewId, reactionType);

            // Trả về JSON để Javascript xử lý
            return Ok(new { newCount = newCount, userHasReacted = userHasReacted });
        }
    }
}