using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.Repositories.Interface;
using SocialReview.Services;
using SocialReview.ViewModels;

namespace SocialReview.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly IReviewRepository _reviewRepo;
        private readonly UserManager<User> _userManager;
        private readonly ISlugService _slugService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductRepository productRepo,IReviewRepository reviewRepo,UserManager<User> userManager,ApplicationDbContext context,ISlugService slugService,IWebHostEnvironment  webHostEnvironment) // Thêm DbContext
        {
            _reviewRepo = reviewRepo;
            _productRepo = productRepo;
            _userManager = userManager;
            _context = context;
            _slugService = slugService;
            _webHostEnvironment = webHostEnvironment;
        }

        // (Hàm này có thể đặt trong Controller hoặc một Service riêng)
        

        

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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReviewDetails(int id)
        {
            var review = await _reviewRepo.GetByIdAsync(id); // Giả sử bạn có hàm này
            if (review == null)
            {
                return NotFound();
            }

            // KIỂM TRA BẢO MẬT: Đảm bảo chỉ chủ sở hữu mới lấy được
            var currentUserId = _userManager.GetUserId(User);
            if (review.UserId.ToString() != currentUserId)
            {
                return Forbid(); // Trả về lỗi 403
            }

            // Trả về JSON chỉ các trường cần thiết
            return Ok(new
            {
                reviewID = review.ReviewID,
                title = review.Title,
                content = review.Content,
                rating = review.Rating
            });
        }

        /// <summary>
        /// [POST] Cập nhật thông tin review sau khi người dùng sửa
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateReview(
            [FromForm] int ReviewID,
            [FromForm] string Title,
            [FromForm] string Content,
            [FromForm] int Rating)
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content) || Rating < 1 || Rating > 5)
            {
                return BadRequest(new { errors = new[] { "Vui lòng điền đầy đủ thông tin." } });
            }

            // Tìm review gốc
            var reviewToUpdate = await _reviewRepo.GetByIdAsync(ReviewID);
            if (reviewToUpdate == null)
            {
                return NotFound();
            }

            // KIỂM TRA BẢO MẬT: (Rất quan trọng)
            // Phải kiểm tra lại quyền sở hữu ở server
            var currentUserId = _userManager.GetUserId(User);
            if (reviewToUpdate.UserId.ToString() != currentUserId)
            {
                return Forbid(); // Lỗi 403
            }

            // Cập nhật thông tin
            reviewToUpdate.Title = Title;
            reviewToUpdate.Content = Content;
            reviewToUpdate.Rating = Rating;
            // (Thêm logic cập nhật ảnh nếu muốn)

            await _reviewRepo.UpdateAsync(reviewToUpdate); // Giả sử bạn có hàm này

            // Trả về dữ liệu mới để cập nhật UI
            return Ok(new
            {
                success = true,
                message = "Cập nhật thành công!",
                reviewID = reviewToUpdate.ReviewID,
                newTitle = reviewToUpdate.Title,
                newContent = reviewToUpdate.Content,
                newRating = reviewToUpdate.Rating
            });
        }
        public async Task<IActionResult> Detail(string slug) // <-- Thay 'int id' bằng 'string slug'
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest();
            }

            var product = await _context.Products
        .Include(p => p.Company) // Tải thông tin công ty
        .Include(p => p.Reviews) // Tải danh sách review
            .ThenInclude(r => r.User) // Tải User của mỗi review
        .Include(p => p.Reviews)
            .ThenInclude(r => r.Reactions) // Tải Reactions của mỗi review
        .Include(p => p.Reviews)
            .ThenInclude(r => r.Comments) // <-- QUAN TRỌNG: Tải Comments của mỗi review
                .ThenInclude(c => c.User) // Tải User của mỗi comment
        .Include(p => p.Reviews)
            .ThenInclude(r => r.Comments)
                .ThenInclude(c => c.Reactions) // (Nếu bạn có reaction cho comment)
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Slug == slug);


            if (product == null) { return NotFound(); }

            // Toàn bộ logic ViewModel còn lại giữ nguyên
            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                Reviews = product.Reviews ?? new List<Review>(),
                NewReviewForm = new CreateReviewViewModel { ProductId = product.ProductID }, // Vẫn dùng ID ở đây
                TotalReviews = product.Reviews?.Count() ?? 0,
                AverageRating = (product.Reviews != null && product.Reviews.Any())
                                ? product.Reviews.Average(r => r.Rating)
                                : 0
            };

            return View(viewModel);
        }

        private async Task<string> SaveFileAsync(IFormFile file, string subfolder)
        {
            // 1. Tạo đường dẫn thư mục (ví dụ: wwwroot/images/reviews)
            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images", subfolder);
            Directory.CreateDirectory(uploadDir); // Tạo thư mục nếu chưa tồn tại

            // 2. Tạo tên file độc nhất
            string fileExtension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // 3. Đường dẫn lưu file vật lý
            string filePath = Path.Combine(uploadDir, uniqueFileName);

            // 4. Lưu file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 5. Trả về đường dẫn web (URL)
            return $"/images/{subfolder}/{uniqueFileName}";
        }

        // ... (các using và constructor giữ nguyên)

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReview(
             [Bind(Prefix = "NewReviewForm")] CreateReviewViewModel model) // <-- Dùng ViewModel đã cập nhật
        {
            if (ModelState.IsValid)
            {
                var userIdString = _userManager.GetUserId(User);
                int.TryParse(userIdString, out int userIdInt);

                // 1. XỬ LÝ UPLOAD ẢNH (NẾU CÓ)
                string? uniqueImageUrl = null; // Biến lưu URL ảnh
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Lưu file vào server (ví dụ: wwwroot/images/reviews)
                    uniqueImageUrl = await SaveFileAsync(model.ImageFile, "reviews");
                }

                // 2. TẠO REVIEW (Bao gồm cả URL ảnh)
                var review = new Review
                {
                    Title = model.Title,
                    Content = model.Content,
                    Rating = model.Rating,
                    ProductID = model.ProductId,
                    UserId = userIdInt,
                    CreatedAt = DateTime.UtcNow,
                    Image = uniqueImageUrl // <-- GÁN URL ẢNH DUY NHẤT VÀO ĐÂY 
                };

                // 3. LƯU REVIEW VÀO DB
                await _reviewRepo.AddAsync(review);

                // 4. TRẢ VỀ PARTIAL VIEW
                var user = await _userManager.GetUserAsync(User);
                review.User = user;

                return PartialView("~/Views/Shared/_ReviewCardPartial.cshtml", review);
            }

            // --- XỬ LÝ LỖI (giữ nguyên) ---
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage);

            return BadRequest(new { errors = errors });
        }

        // Hàm SaveFileAsync(IFormFile file, string subfolder) giữ nguyên
    }
}
