using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialReview.Models;
using SocialReview.Repositories.Interface; // Nhớ đổi interface

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentReactionController : ControllerBase
{
    private readonly ICommentReactionRepository _reactionRepo;
    private readonly UserManager<User> _userManager;

    public CommentReactionController(ICommentReactionRepository reactionRepo, UserManager<User> userManager)
    {
        _reactionRepo = reactionRepo;
        _userManager = userManager;
    }

    [HttpPost("toggle")]
    public async Task<IActionResult> ToggleReaction([FromForm] int commentId, [FromForm] string reactionType)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        bool userHasReacted = await _reactionRepo.ToggleReactionAsync(commentId, user.Id, reactionType);
        int newCount = await _reactionRepo.GetReactionCountAsync(commentId, reactionType);

        return Ok(new { newCount, userHasReacted });
    }
}