using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialReview.Models;
using SocialReview.Repositories.Interface;
using SocialReview.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SocialReview.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly UserManager<User> _userManager;

        public UserController(IUserRepository userRepo,UserManager<User> userManager)
        {
            _userRepo = userRepo;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
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


            //return View(viewModel);
            var username = _userManager.GetUserName(User);
            if (string.IsNullOrEmpty(username))
            {
                // Nếu không có username (lỗi lạ), quay về trang chủ
                return RedirectToAction("Index", "Home");
            }
            var user = await _userRepo.GetUserByUsername(username);

            if (user == null)
            {
                // Lỗi không tìm thấy user (dù đã đăng nhập??)
                return NotFound("Không tìm thấy tài khoản.");
            }
            var viewModel = new UserInfoViewModel
            {
                // Giả sử ViewModel của bạn có thuộc tính UserProfile
                UserProfile = user,
                // TODO: Lấy thêm danh sách review của user này
                // UserReviews = await _reviewRepo.GetReviewsByUserIdAsync(user.Id)
            };


            // Trả về View "Profile.cshtml" (tệp chúng ta đã làm)
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
