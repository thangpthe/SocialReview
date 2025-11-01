using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.Repositories.Interface;
using SocialReview.ViewModels;

namespace SocialReview.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IReviewRepository _reviewRepo;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context; // Dùng để lấy Categories

        public ProductController(IProductRepository productRepo,IReviewRepository reviewRepo,UserManager<User> userManager,ApplicationDbContext context) // Thêm DbContext
        {
            _reviewRepo = reviewRepo;
            _productRepo = productRepo;
            _userManager = userManager;
            _context = context; // Gán DbContext
        }

        // --- 1. ACTION MỚI: Trang Lọc Sản phẩm ---
        // GET: /Product/Index
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? productType, int? rating)
        {
            // 1. Lấy kết quả sản phẩm đã lọc
            var products = await _productRepo.FilterAsync(productType, rating);

            // 2. Tạo "khay" ViewModel
            var viewModel = new ProductSearchViewModel
            {
                Products = products,
                CurrentType = productType,
                CurrentRating = rating

            };
            return View(viewModel);
        }

        // Trong file: /Controllers/ProductController.cs
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productRepo.GetProductDetailById(id);
            if (product == null) { return NotFound(); }

            var viewModel = new ProductDetailViewModel
            {
                Product = product,

                Reviews = product.Reviews ?? new List<Review>(),

                // Các thông tin khác...
                NewReviewForm = new CreateReviewViewModel { ProductId = id },
                TotalReviews = product.Reviews?.Count() ?? 0,
                AverageRating = (product.Reviews != null && product.Reviews.Any())
                                ? product.Reviews.Average(r => r.Rating)
                                : 0
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userIdString = _userManager.GetUserId(User);

                // (Giả sử bạn đã sửa CSDL dùng int UserId)
                int.TryParse(userIdString, out int userIdInt);

                var review = new Review
                {
                    Title = model.Title,
                    Content = model.Content,
                    Rating = model.Rating,
                    ProductID = model.ProductId,
                    UserId = userIdInt, // Lấy từ server, không phải từ form
                    CreatedAt = DateTime.UtcNow
                };

                // Dùng ReviewRepository (đã sửa) để lưu CSDL
                await _reviewRepo.AddAsync(review);

                // Quay lại trang chi tiết sản phẩm
                return RedirectToAction("Detail", new { id = model.ProductId });
            }

            // --- XỬ LÝ TRƯỜNG HỢP FORM LỖI (Unhappy Path) ---
            // Nếu form không hợp lệ (ví dụ: thiếu Title, Rating > 5...)
            // Chúng ta phải tải lại toàn bộ dữ liệu của trang Detail

            var product = await _productRepo.GetProductDetailById(model.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            // Tạo lại ViewModel
            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                Reviews = product.Reviews ?? new List<Review>(),
                TotalReviews = product.Reviews?.Count() ?? 0,
                AverageRating = (product.Reviews != null && product.Reviews.Any())
                                ? product.Reviews.Average(r => r.Rating)
                                : 0,

                
                NewReviewForm = model
            };

            // Trả về View "Detail" (chứ không phải "CreateReview")
            return View("Detail", viewModel);
        }
    }
}
