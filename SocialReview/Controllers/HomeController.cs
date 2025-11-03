using Microsoft.AspNetCore.Mvc;
using SocialReview.Models;
using SocialReview.Repositories.Interface; // <-- (Xóa Using này)
using SocialReview.Services;
using SocialReview.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SocialReview.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;
        // XÓA: private readonly IReviewRepository _reviewRepo;

        public HomeController(IDashboardService dashboardService) // <-- Sửa: Xóa IReviewRepository
        {
            _dashboardService = dashboardService;
            // XÓA: _reviewRepo = reviewRepo;
        }

        // --- SỬA LẠI ACTION INDEX ---
        public async Task<IActionResult> Index()
        {
            // Lấy thông tin thống kê (Total Users, Companies...)
            var statsViewModel = await _dashboardService.GetDashboardStatsAsync();

            // Gửi "khay" thống kê đến View
            return View(statsViewModel);
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

