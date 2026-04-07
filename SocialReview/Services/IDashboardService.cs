using SocialReview.ViewModels;

namespace SocialReview.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardStatsAsync();
    }
}
