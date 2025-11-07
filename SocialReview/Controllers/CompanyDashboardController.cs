using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System; // Thêm System (nếu chưa có)
using System.Linq;
using SocialReview.Services; // Thêm Linq (nếu chưa có)

namespace SocialReview.Controllers
{
    [Authorize(Roles = "Company")]
    public class CompanyDashboardController : Controller
    {
        private readonly UserManager<User> _userManager; 
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyDashboardController> _logger;
        private readonly ISlugService _slugService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // SỬA LỖI: Đã sửa lại constructor
        public CompanyDashboardController(UserManager<User> userManager,ApplicationDbContext context,ILogger<CompanyDashboardController> logger,IWebHostEnvironment webHostEnvironment,ISlugService slugService)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _slugService = slugService;

        }

        //[HttpGet]
        //[Authorize(Roles = "Company")] // Đảm bảo chỉ admin/company mới chạy được
        //public async Task<IActionResult> UpdateExistingSlugs()
        //{
        //    _logger.LogInformation("Bắt đầu tác vụ cập nhật slug cho sản phẩm cũ...");

        //    // 1. Lấy TẤT CẢ sản phẩm đang KHÔNG CÓ (hoặc slug rỗng)
        //    var productsToUpdate = await _context.Products
        //                                .Where(p => string.IsNullOrEmpty(p.Slug))
        //                                .ToListAsync();

        //    if (!productsToUpdate.Any())
        //    {
        //        _logger.LogInformation("Không tìm thấy sản phẩm nào cần cập nhật slug.");
        //        return Content("Tất cả sản phẩm đã có slug. Không có gì để cập nhật.");
        //    }

        //    int count = 0;
        //    // 2. Lặp qua từng sản phẩm
        //    foreach (var product in productsToUpdate)
        //    {
        //        // 3. Tạo slug mới từ tên sản phẩm
        //        product.Slug = _slugService.GenerateSlug(product.ProductName);
        //        count++;
        //    }

        //    // 4. Lưu tất cả thay đổi vào DB
        //    await _context.SaveChangesAsync();

        //    string message = $"Hoàn tất: Đã cập nhật slug thành công cho {count} sản phẩm.";
        //    _logger.LogInformation(message);

        //    // 5. Trả về thông báo
        //    return Content(message);
        //}
        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdString);
        }

        private async Task<int?> GetCurrentCompanyIdAsync()
        {
            var currentUserId = GetCurrentUserId();
            var company = await _context.Companies
                                .AsNoTracking()
                                .FirstOrDefaultAsync(c => c.UserID == currentUserId);
            return company?.CompanyID;
        }

        // (Hàm SaveFileAsync giữ nguyên)
        private async Task<string> SaveFileAsync(IFormFile file, string subfolder)
        {
            // 1. Tạo đường dẫn thư mục (ví dụ: wwwroot/images/logos)
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

        // (Index action - Cập nhật để load Categories)
        public async Task<IActionResult> Index()
        {
            var currentUserId = GetCurrentUserId();
            var companyProfile = await _context.Companies.FirstOrDefaultAsync(c => c.UserID == currentUserId);
            if (companyProfile == null)
            {
                return RedirectToAction("CreateProfile");
            }

            // SỬA LỖI: Load CategoryName khi lấy danh sách Product
            var myProducts = await _context.Products
                .Where(p => p.CompanyID == companyProfile.CompanyID)
                .Include(p => p.Category) // <-- QUAN TRỌNG: Include Category
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            var viewModel = new CompanyViewModel
            {
                CompanyProfile = companyProfile,
                CompanyProducts = myProducts
            };

            // Lấy danh sách Categories cho Modal (nay là cho trang mới)
            ViewData["Categories"] = await _context.Categories.ToListAsync();
            return View(viewModel);
        }

        // (EditProfile GET/POST và CreateProfile GET/POST giữ nguyên)
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var currentUserId = GetCurrentUserId();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.UserID == currentUserId);
            if (company == null)
            {
                return RedirectToAction(nameof(CreateProfile));
            }

            var viewModel = new CompanyEditViewModel
            {
                CompanyName = company.CompanyName,
                Logo = company.Logo, // Truyền Logo URL hiện tại
                CompanyDescription = company.CompanyDescription,
                Industry = company.Industry,
                Website = company.Website,
                ContactEmail = company.ContactEmail,
                Phone = company.Phone
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(CompanyEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUserId = GetCurrentUserId();
            var companyToUpdate = await _context.Companies.FirstOrDefaultAsync(c => c.UserID == currentUserId);
            if (companyToUpdate == null)
            {
                return NotFound("Không tìm thấy hồ sơ doanh nghiệp.");
            }

            companyToUpdate.CompanyName = model.CompanyName;
            companyToUpdate.CompanyDescription = model.CompanyDescription;
            companyToUpdate.Industry = model.Industry;
            companyToUpdate.Website = model.Website;
            companyToUpdate.ContactEmail = model.ContactEmail;
            companyToUpdate.Phone = model.Phone;

            if (model.LogoFile != null && model.LogoFile.Length > 0)
            {
                string newLogoUrl = await SaveFileAsync(model.LogoFile, "logos");
                companyToUpdate.Logo = newLogoUrl;
            }

            try
            {
                _context.Update(companyToUpdate);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Không thể lưu thay đổi.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateProfile()
        {
            var currentUserId = GetCurrentUserId();
            bool hasProfile = await _context.Companies.AnyAsync(c => c.UserID == currentUserId);
            if (hasProfile) return RedirectToAction(nameof(Index));

            var user = await _userManager.FindByIdAsync(currentUserId.ToString());
            var viewModel = new CompanyEditViewModel
            {
                CompanyName = user.FullName,
                ContactEmail = user.Email
            };
            ViewData["IsCreating"] = true;
            return View("EditProfile", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(CompanyEditViewModel model)
        {
            var currentUserId = GetCurrentUserId();
            bool hasProfile = await _context.Companies.AnyAsync(c => c.UserID == currentUserId);
            if (hasProfile) return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid)
            {
                ViewData["IsCreating"] = true;
                return View("EditProfile", model);
            }

            var user = await _userManager.FindByIdAsync(currentUserId.ToString());
            var newCompany = new Company
            {
                UserID = currentUserId,
                CompanyName = model.CompanyName,
                CompanyDescription = model.CompanyDescription,
                Industry = model.Industry,
                Website = model.Website,
                ContactEmail = model.ContactEmail ?? user.Email,
                Phone = model.Phone
            };

            if (model.LogoFile != null && model.LogoFile.Length > 0)
            {
                string newLogoUrl = await SaveFileAsync(model.LogoFile, "logos");
                newCompany.Logo = newLogoUrl;
            }

            _context.Companies.Add(newCompany);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tạo hồ sơ doanh nghiệp thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ==================================================================
        // CẬP NHẬT CHỨC NĂNG SẢN PHẨM (BẮT ĐẦU)
        // ==================================================================

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            ViewData["Categories"] = await _context.Categories.ToListAsync();
            var viewModel = new ProductCRUDViewModel();
            return View("ProductForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCRUDViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin doanh nghiệp.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model CreateProduct không hợp lệ.");
                ViewData["Categories"] = await _context.Categories.ToListAsync();
                return View("ProductForm", model);
            }

            try
            {
                var newProduct = new Product
                {
                    CompanyID = companyId.Value,
                    CategoryID = model.CategoryID,
                    ProductName = model.ProductName,
                    ProductDescription = model.ProductDescription,
                    ProductPrice = model.ProductPrice,
                    ProductType = model.ProductType,
                    CreatedAt = DateTime.Now,
                    Slug = _slugService.GenerateSlug(model.ProductName)
                };

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string newImageUrl = await SaveFileAsync(model.ImageFile, "products");
                    newProduct.ProductImage = newImageUrl;
                }

                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Tạo sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sản phẩm mới (Form)");
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi lưu: " + ex.Message);
                ViewData["Categories"] = await _context.Categories.ToListAsync();
                return View("ProductForm", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductID == id && p.CompanyID == companyId.Value);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm hoặc bạn không có quyền sửa.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new ProductCRUDViewModel
            {
                ProductID = product.ProductID,
                CategoryID = product.CategoryID,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductImage = product.ProductImage,
                ProductPrice = product.ProductPrice,
                ProductType = product.ProductType
            };

            ViewData["Categories"] = await _context.Categories.ToListAsync();
            return View("ProductForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, [FromForm] ProductCRUDViewModel model)
        {
            if (id != model.ProductID)
            {
                return BadRequest();
            }

            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model EditProduct không hợp lệ.");
                ViewData["Categories"] = await _context.Categories.ToListAsync();
                return View("ProductForm", model);
            }

            try
            {
                var productToUpdate = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductID == id && p.CompanyID == companyId.Value);

                if (productToUpdate == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm hoặc bạn không có quyền sửa.";
                    return RedirectToAction(nameof(Index));
                }

                productToUpdate.CategoryID = model.CategoryID;
                productToUpdate.ProductName = model.ProductName;
                productToUpdate.ProductDescription = model.ProductDescription;
                productToUpdate.ProductPrice = model.ProductPrice;
                productToUpdate.ProductType = model.ProductType;
                productToUpdate.Slug = _slugService.GenerateSlug(model.ProductName);
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string newImageUrl = await SaveFileAsync(model.ImageFile, "products");
                    productToUpdate.ProductImage = newImageUrl;
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm ID: {Id} (Form)", id);
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi lưu: " + ex.Message);
                ViewData["Categories"] = await _context.Categories.ToListAsync();
                return View("ProductForm", model);
            }
        }

        // GIỮ NGUYÊN Action 'DeleteProduct' (vẫn dùng AJAX)
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var companyId = await GetCurrentCompanyIdAsync();
                var productToDelete = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductID == id && p.CompanyID == companyId.Value);

                if (productToDelete == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm hoặc bạn không có quyền xóa." });
                }

                // (Tùy chọn: Thêm logic xóa file ảnh của sản phẩm này)
                // ...

                _context.Products.Remove(productToDelete);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa sản phẩm thành công." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm ID: {Id}", id);
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }
        // ==================================================================
        // CẬP NHẬT CHỨC NĂNG SẢN PHẨM (KẾT THÚC)
        // ==================================================================
    }
}