using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialReview.Models;
using SocialReview.Repositories.Interface;
using SocialReview.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SocialReview.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly UserManager<User> _userManager;

        public UserController(IUserRepository userRepo,UserManager<User> userManager)
        {
            _userRepo = userRepo;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string username)
        {
            //var username = _userManager.GetUserName(User);
            //var user = _userRepo.GetUserByUsername(username);
            //if (user == null)
            //{

            //    ViewData["ErrorMessage"] = "Không tìm thấy hồ sơ doanh nghiệp cho tài khoản này. Vui lòng tạo hồ sơ.";
            //    return View("Error"); // Bạn cần tạo 1 View Error chung
            //}
            //var viewModel = new UserInfoViewModel
            //{
            //    UserProfile = user
            //};


            if (string.IsNullOrEmpty(username))
            {
                // Nếu không có username, quay về trang chủ
                return RedirectToAction("Index", "Home");
            }

            // 5. SỬ DỤNG USERNAME TỪ THAM SỐ
            var user = await _userRepo.GetUserReview(username);

            if (user == null)
            {
                return NotFound("Không tìm thấy tài khoản.");
            }

            var viewModel = new UserInfoViewModel
            {
                UserProfile = user,
                UserReviews = user.Reviews ?? new List<Review>()
            };

            return View(viewModel);
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
