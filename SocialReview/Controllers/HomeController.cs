using Microsoft.AspNetCore.Mvc;
using SocialReview.Models;
using SocialReview.Repositories.Interface; // BẮT BUỘC PHẢI GIỮ LẠI ĐỂ DÙNG IReviewRepository
using SocialReview.Services;
using SocialReview.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SocialReview.Controllers
{
    public class HomeController : Controller
    {
        //private readonly IDashboardService _dashboardService;
        private readonly IReviewRepository _reviewRepo;

        // BẮT BUỘC PHẢI GIỮ LẠI IReviewRepository TRONG CONSTRUCTOR
        public HomeController(IDashboardService dashboardService, IReviewRepository reviewRepo)
        {
            //_dashboardService = dashboardService;
            _reviewRepo = reviewRepo;
        }

        public async Task<IActionResult> Index()
        {
            // 1. MỞ LẠI CODE LẤY THÔNG TIN THỐNG KÊ (Vì View của bạn cần Model để hiển thị số lượng)
            //var statsViewModel = await _dashboardService.GetDashboardStatsAsync();

            // 2. LẤY 3 REVIEW MỚI NHẤT TỪ DATABASE
            ViewBag.LatestReviews = await _reviewRepo.GetLatestReviewsAsync(3);

            // 3. GỬI BIẾN statsViewModel RA NGOÀI VIEW
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}