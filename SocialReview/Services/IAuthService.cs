
using Microsoft.AspNetCore.Identity;
using SocialReview.ViewModels;
namespace SocialReview.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<string?> LoginAsync(string username, string password);
        Task LogoutAsync();
    }
}