using Microsoft.AspNetCore.Mvc;
using SocialReview.Repositories.Interface;

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
            // Xử lý query null hoặc chỉ chứa khoảng trắng
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index", "Home");
            }

            // Gọi repository để tìm
            var results = await _productRepo.Search(query);

            // Gửi query đã được dọn sạch khoảng trắng ra View
            ViewData["SearchQuery"] = query.Trim();

            return View(results);
        }
    }
}