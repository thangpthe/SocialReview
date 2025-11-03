using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialReview.Models;
using SocialReview.Repositories.Interface;
using SocialReview.ViewModels;

namespace SocialReview.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly UserManager<User> _userManager;

        public CompanyController(ICompanyRepository companyRepo, UserManager<User> userManager)
        {
            _companyRepo = companyRepo;
            _userManager = userManager;
        }
        public async Task <IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if(!int.TryParse(userId, out int userIdInt))
            {
                ViewData["ErrorMessage"] = "Tài khoản không hợp lệ.";
                return View("Error");
            }
            var company = await _companyRepo.GetCompanyByUserIdAsync(userIdInt);
            if (company == null)
            {
                
                ViewData["ErrorMessage"] = "Không tìm thấy hồ sơ doanh nghiệp cho tài khoản này. Vui lòng tạo hồ sơ.";
                return View("Error"); // Bạn cần tạo 1 View Error chung
            }
            var viewModel = new CompanyViewModel
            {
                CompanyProfile = company,
                CompanyProducts = company.Products ?? new List<Product>() // Đảm bảo Products không bị null
            };

            // 6. Gửi "khay" đến View (Views/Company/Dashboard.cshtml)
            return View(viewModel);
        }
    }
}
