
using Microsoft.AspNetCore.Identity;
using SocialReview.Models;
using SocialReview.ViewModels;

namespace SocialReview.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.Username,
                Email = model.UserEmail,
                FullName = model.FullName,
                UserRole = model.UserRole,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            return result;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);
            return result.Succeeded;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}