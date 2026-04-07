// Trong file: SearchController.cs

using Microsoft.AspNetCore.Mvc;
using SocialReview.Repositories.Interface;
using SocialReview.ViewModels;

namespace SocialReview.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProductRepository _productRepo;
        public SearchController(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }
        public async Task<IActionResult> Index(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                // Nếu tìm kiếm rỗng, quay về trang chủ
                return RedirectToAction("Index", "Home");
            }

            // Gọi repository để tìm
            var results = await _productRepo.Search(query);

            // Lưu lại từ khóa để View có thể hiển thị: "Kết quả cho '...'"
            ViewData["SearchQuery"] = query;

            // Gửi danh sách (List<Product>) đến View
            // (File Views/Search/Index.cshtml của bạn đã sẵn sàng nhận cái này)
            return View(results);
        }
    }
}