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
        private readonly ApplicationDbContext _context;

        public ProductController(IProductRepository productRepo,IReviewRepository reviewRepo,UserManager<User> userManager,ApplicationDbContext context) // Thêm DbContext
        {
            _reviewRepo = reviewRepo;
            _productRepo = productRepo;
            _userManager = userManager;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? CurrentType, int? CurrentRating)
        {
            //var products = await _productRepo.FilterAsync(productType, rating);

            //// Lấy danh sách các loại sản phẩm duy nhất từ database
            //var typeOptions = await _context.Products
            //    .Select(p => p.ProductType)
            //    .Distinct()
            //    .Where(t => !string.IsNullOrEmpty(t))
            //    .OrderBy(t => t)
            //    .ToListAsync();

            //var viewModel = new ProductSearchViewModel
            //{
            //    Products = products,
            //    CurrentType = productType,
            //    CurrentRating = rating,
            //    TypeOptions = typeOptions,
            //    RatingOptions = new List<int> { 1, 2, 3, 4, 5 }
            //};

            //return View(viewModel);
            Console.WriteLine($"Index called - ProductType: {CurrentType ?? "null"}, Rating: {CurrentRating?.ToString() ?? "null"}");

            // Gọi FilterAsync với parameters
            var products = await _productRepo.FilterAsync(CurrentType, CurrentRating);

            // Lấy danh sách loại sản phẩm từ database
            var typeOptions = await _context.Products
                .Select(p => p.ProductType)
                .Distinct()
                .Where(t => !string.IsNullOrEmpty(t))
                .OrderBy(t => t)
                .ToListAsync();

            var viewModel = new ProductSearchViewModel
            {
                Products = products,
                CurrentType =CurrentType,
                CurrentRating = CurrentRating,
                TypeOptions = typeOptions,
                RatingOptions = new List<int> { 1, 2, 3, 4, 5 }
            };

            return View(viewModel);
        }
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(
             [Bind(Prefix = "NewReviewForm")] CreateReviewViewModel model)
        {
            // --- XỬ LÝ TRƯỜNG HỢP FORM HỢP LỆ (Happy Path) ---
            if (ModelState.IsValid)
            {
                var userIdString = _userManager.GetUserId(User);
                int.TryParse(userIdString, out int userIdInt);

                var review = new Review
                {
                    Title = model.Title,
                    Content = model.Content,
                    Rating = model.Rating,
                    ProductID = model.ProductId,
                    UserId = userIdInt,
                    CreatedAt = DateTime.UtcNow,
                    //Status = "Pending"
                };

                await _reviewRepo.AddAsync(review);

                var user = await _userManager.GetUserAsync(User);
                review.User = user;

                return PartialView("~/Views/Shared/_ReviewCardPartial.cshtml", review);
            }

            // --- XỬ LÝ TRƯỜNG HỢP FORM LỖI (Unhappy Path) ---
            // (Nếu Model Binding thất bại, ModelState.IsValid sẽ là false)
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage);

            // Trả về lỗi 400 (BadRequest) kèm danh sách lỗi
            return BadRequest(new { errors = errors });
        }
    }
}
