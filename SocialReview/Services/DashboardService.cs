using SocialReview.Data;
using SocialReview.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace SocialReview.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardStatsAsync()
        {
           

            // Chờ tất cả hoàn thành
            var stats = new DashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalCompanies = await _context.Companies.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalReviews = await _context.Reviews.CountAsync()
            };

            return stats;
        }
    }
}
