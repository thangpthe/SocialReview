using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
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
        private readonly ApplicationDbContext _context;
        // XÓA: private readonly IReviewRepository _reviewRepo;

        public HomeController(IDashboardService dashboardService,ApplicationDbContext context) // <-- Sửa: Xóa IReviewRepository
        {
            _dashboardService = dashboardService;
            _context = context;
            // XÓA: _reviewRepo = reviewRepo;
        }

        // --- SỬA LẠI ACTION INDEX ---
        //public async Task<IActionResult> Index()
        //{
        //    // Lấy thông tin thống kê (Total Users, Companies...)
        //    var statsViewModel = await _dashboardService.GetDashboardStatsAsync();

        //    // Gửi "khay" thống kê đến View
        //    return View(statsViewModel);
        //}

        public async Task<IActionResult> Index()
        {
            // Lấy các thống kê (giống như bạn đang làm)
            var totalUsers = await _context.Users.CountAsync();
            var totalCompanies = await _context.Companies.CountAsync(); // Giả sử bạn có DbSet<Company>
            var totalProducts = await _context.Products.CountAsync(); // Giả sử bạn có DbSet<Product>
            var totalReviews = await _context.Reviews.CountAsync();

            // === PHẦN MỚI: LẤY REVIEW GẦN ĐÂY ===
            // Lấy 5 bài review mới nhất,
            // và dùng .Include(r => r.User) để tải thông tin User (từ Review.cs)
            var recentReviews = await _context.Reviews
                                        .Include(r => r.User) // Tải thông tin User liên quan
                                        .OrderByDescending(r => r.CreatedAt) // Sắp xếp mới nhất trước
                                        .Take(5) // Chỉ lấy 5 bài
                                        .ToListAsync();
            // === KẾT THÚC PHẦN MỚI ===

            // Tạo ViewModel
            var viewModel = new DashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalCompanies = totalCompanies,
                TotalProducts = totalProducts,
                TotalReviews = totalReviews,
                RecentReviews = recentReviews // Thêm danh sách review vào ViewModel
            };

            return View(viewModel);
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

