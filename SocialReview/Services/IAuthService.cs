
using SocialReview.ViewModels;
using SocialReview.ViewModels;
namespace SocialReview.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterViewModel model);
        Task<bool> LoginAsync(string username, string password);
        Task LogoutAsync();
    }
}