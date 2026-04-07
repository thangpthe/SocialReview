using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.ViewModels;
using System.Security.Claims;

namespace SocialReview.Controllers
{
    [Authorize(Roles = "Company")]
    public class CompanyDashboardController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyDashboardController> _logger;


        public CompanyDashboardController(UserManager<User> userManager, ApplicationDbContext context, ILogger<CompanyDashboardController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // --- Hàm helper để lấy ID của user hiện tại ---
        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdString);
        }


        private async Task<int?> GetCurrentCompanyIdAsync()
        {
            var currentUserId = GetCurrentUserId();
            // AsNoTracking() giúp truy vấn nhanh hơn vì không cần theo dõi thay đổi
            var company = await _context.Companies
                                .AsNoTracking()
                                .FirstOrDefaultAsync(c => c.UserID == currentUserId);
            return company?.CompanyID;
        }
        //
        // GET: /CompanyDashboard/Index (hoặc /CompanyDashboard)
        //
        public async Task<IActionResult> Index()
        {
            var currentUserId = GetCurrentUserId();

            // 1. Lấy thông tin Doanh nghiệp (Company) dựa trên UserID
            var companyProfile = await _context.Companies.FirstOrDefaultAsync(c => c.UserID == currentUserId);

            if (companyProfile == null)
            {
                return RedirectToAction("CreateProfile");
            }

            // 2. Lấy danh sách sản phẩm của Doanh nghiệp này
            var myProducts = await _context.Products
                .Where(p => p.CompanyID == companyProfile.CompanyID)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            // 3. Tạo ViewModel
            var viewModel = new CompanyViewModel
            {
                CompanyProfile = companyProfile,
                CompanyProducts = myProducts
            };
            //ViewData["Categories"] = await _context.Categories.ToListAsync();
            return View(viewModel);
        }

        // GET: /CompanyDashboard/EditProfile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var currentUserId = GetCurrentUserId();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.UserID == currentUserId);

            if (company == null)
            {
                return RedirectToAction(nameof(CreateProfile));
            }

            // SỬA LỖI: Map từ Model 'Company' sang 'CompanyEditViewModel'
            var viewModel = new CompanyEditViewModel
            {
                CompanyName = company.CompanyName,
                Logo = company.Logo,
                CompanyDescription = company.CompanyDescription,
                Industry = company.Industry,
                Website = company.Website,
                ContactEmail = company.ContactEmail,
                Phone = company.Phone
            };

            return View(viewModel); // Gửi CompanyEditViewModel đến View
        }

        //
        // POST: /CompanyDashboard/EditProfile
        // Xử lý dữ liệu form được gửi lên
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(CompanyEditViewModel model) // SỬA LỖI: Dùng CompanyEditViewModel
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Trả về form với các lỗi
            }

            var currentUserId = GetCurrentUserId();

            // Lấy Company gốc từ CSDL
            var companyToUpdate = await _context.Companies.FirstOrDefaultAsync(c => c.UserID == currentUserId);

            if (companyToUpdate == null)
            {
                return NotFound("Không tìm thấy hồ sơ doanh nghiệp.");
            }

            // SỬA LỖI: Cập nhật thông tin từ ViewModel an toàn
            companyToUpdate.CompanyName = model.CompanyName;
            companyToUpdate.Logo = model.Logo;
            companyToUpdate.CompanyDescription = model.CompanyDescription;
            companyToUpdate.Industry = model.Industry;
            companyToUpdate.Website = model.Website;
            companyToUpdate.ContactEmail = model.ContactEmail;
            companyToUpdate.Phone = model.Phone;

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



        // GET: /CompanyDashboard/CreateProfile
        [HttpGet]
        public async Task<IActionResult> CreateProfile()
        {
            var currentUserId = GetCurrentUserId();

            bool hasProfile = await _context.Companies.AnyAsync(c => c.UserID == currentUserId);
            if (hasProfile)
            {
                return RedirectToAction(nameof(Index)); // Nếu có rồi, về trang Index
            }

            var user = await _userManager.FindByIdAsync(currentUserId.ToString());

            // Điền sẵn tên và email
            var viewModel = new CompanyEditViewModel
            {
                CompanyName = user.FullName, // Giả định FullName là tên công ty
                ContactEmail = user.Email
            };

            ViewData["IsCreating"] = true;
            return View("EditProfile", viewModel); // Tái sử dụng View "EditProfile.cshtml"
        }

        // POST: /CompanyDashboard/CreateProfile
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(CompanyEditViewModel model)
        {
            var currentUserId = GetCurrentUserId();
            bool hasProfile = await _context.Companies.AnyAsync(c => c.UserID == currentUserId);
            if (hasProfile)
            {
                return RedirectToAction(nameof(Index)); // Đã có, không tạo mới
            }

            if (!ModelState.IsValid)
            {
                ViewData["IsCreating"] = true;
                return View("EditProfile", model);
            }

            var user = await _userManager.FindByIdAsync(currentUserId.ToString());

            var newCompany = new Company
            {
                UserID = currentUserId, // Gán UserID
                CompanyName = model.CompanyName,
                Logo = model.Logo,
                CompanyDescription = model.CompanyDescription,
                Industry = model.Industry,
                Website = model.Website,
                ContactEmail = model.ContactEmail ?? user.Email,
                Phone = model.Phone
            };

            _context.Companies.Add(newCompany);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tạo hồ sơ doanh nghiệp thành công!";
            return RedirectToAction(nameof(Index));
        }
        //
        // POST: /CompanyDashboard/CreateProduct
        //
        //[HttpPost]
        //[IgnoreAntiforgeryToken]
        //[Consumes("application/json")]
        //public async Task<IActionResult> CreateProduct([FromBody] ProductCRUDViewModel model)
        //{
        //    try
        //    {
        //        var companyId = await GetCurrentCompanyIdAsync();
        //        if (companyId == null)
        //        {
        //            _logger.LogWarning("Không tìm thấy CompanyID cho user hiện tại");
        //            return Json(new { success = false, message = "Không tìm thấy thông tin doanh nghiệp." });
        //        }

        //        // Gán CompanyID của user vào model trước khi kiểm tra
        //        // (Mặc dù chúng ta sẽ gán lại, nhưng validation có thể cần)
        //        if (!ModelState.IsValid)
        //        {
        //            var errors = ModelState.Values
        //                   .SelectMany(v => v.Errors)
        //                   .Select(e => e.ErrorMessage)
        //                   .ToList();

        //            _logger.LogWarning("Model không hợp lệ: {Errors}", string.Join(", ", errors));
        //            return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = errors });
        //        }

        //        // Map từ ViewModel sang Model CSDL
        //        var newProduct = new Product
        //        {
        //            CompanyID = companyId.Value, // Gán CompanyID của user
        //            CategoryID = model.CategoryID,
        //            ProductName = model.ProductName,
        //            ProductDescription = model.ProductDescription,
        //            ProductImage = model.ProductImage,
        //            ProductPrice = model.ProductPrice,
        //            ProductType = model.ProductType,
        //            CreatedAt = DateTime.Now
        //        };

        //        _context.Products.Add(newProduct);
        //        await _context.SaveChangesAsync();

        //        // Tải lại Category để trả về tên
        //        await _context.Entry(newProduct).Reference(p => p.Category).LoadAsync();

        //        // Trả về sản phẩm đã tạo (để JS thêm vào bảng)
        //        return Json(new
        //        {
        //            success = true,
        //            message = "Tạo sản phẩm thành công",
        //            data = newProduct
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi khi tạo sản phẩm mới");
        //        return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
        //    }
        //}

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCRUDViewModel model)
        {
            try
            {
                var companyId = await GetCurrentCompanyIdAsync();
                if (companyId == null)
                {
                    _logger.LogWarning("Không tìm thấy CompanyID cho user hiện tại");
                    return Json(new { success = false, message = "Không tìm thấy thông tin doanh nghiệp." });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    _logger.LogWarning("Model không hợp lệ: {Errors}", string.Join(", ", errors));
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = errors });
                }

                var newProduct = new Product
                {
                    CompanyID = companyId.Value,
                    CategoryID = model.CategoryID,
                    ProductName = model.ProductName,
                    ProductDescription = model.ProductDescription,
                    ProductImage = model.ProductImage,
                    ProductPrice = model.ProductPrice,
                    ProductType = model.ProductType,
                    CreatedAt = DateTime.Now
                };

                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();

                // Tải lại Category để trả về tên
                await _context.Entry(newProduct).Reference(p => p.Category).LoadAsync();

                // SỬA LỖI: Không trả về "newProduct"
                // Trả về một đối tượng "phẳng" mà JavaScript cần
                return Json(new
                {
                    success = true,
                    message = "Tạo sản phẩm thành công",
                    data = new
                    {
                        ProductID = newProduct.ProductID,
                        ProductName = newProduct.ProductName,
                        ProductPrice = newProduct.ProductPrice,
                        ProductType = newProduct.ProductType,
                        CategoryID = newProduct.CategoryID,
                        // Thêm CategoryName để JS có thể dùng ngay
                        CategoryName = newProduct.Category?.CategoryName ?? "Sản phẩm"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo sản phẩm mới");
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProductDetails(int id)
        {
            try
            {
                var companyId = await GetCurrentCompanyIdAsync();
                if (companyId == null) return Forbid();

                var product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ProductID == id && p.CompanyID == companyId.Value);

                if (product == null)
                {
                    return NotFound();
                }

                // Map sang ViewModel để gửi đi
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

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy chi tiết sản phẩm ID: {id}");
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> EditProduct(int id, [FromBody] ProductCRUDViewModel model)
        //{
        //    try
        //    {
        //        _logger.LogInformation("EditProduct được gọi với ID: {Id}, Model: {@Model}", id, model);

        //        if (id != model.ProductID)
        //        {
        //            return Json(new { success = false, message = "ID sản phẩm không khớp." });
        //        }

        //        if (!ModelState.IsValid)
        //        {
        //            var errors = ModelState.Values
        //                .SelectMany(v => v.Errors)
        //                .Select(e => e.ErrorMessage)
        //                .ToList();

        //            _logger.LogWarning("Model không hợp lệ: {Errors}", string.Join(", ", errors));
        //            return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = errors });
        //        }

        //        var companyId = await GetCurrentCompanyIdAsync();
        //        if (companyId == null)
        //        {
        //            return Json(new { success = false, message = "Không tìm thấy thông tin doanh nghiệp." });
        //        }

        //        var productToUpdate = await _context.Products
        //            .FirstOrDefaultAsync(p => p.ProductID == id && p.CompanyID == companyId.Value);

        //        if (productToUpdate == null)
        //        {
        //            return Json(new { success = false, message = "Không tìm thấy sản phẩm hoặc bạn không có quyền sửa." });
        //        }

        //        productToUpdate.CategoryID = model.CategoryID;
        //        productToUpdate.ProductName = model.ProductName;
        //        productToUpdate.ProductDescription = model.ProductDescription;
        //        productToUpdate.ProductImage = model.ProductImage;
        //        productToUpdate.ProductPrice = model.ProductPrice;
        //        productToUpdate.ProductType = model.ProductType;

        //        await _context.SaveChangesAsync();

        //        _logger.LogInformation("Sản phẩm ID {ProductID} được cập nhật thành công", id);

        //        // Load Category để trả về
        //        await _context.Entry(productToUpdate).Reference(p => p.Category).LoadAsync();

        //        return Json(new
        //        {
        //            success = true,
        //            message = "Cập nhật sản phẩm thành công",
        //            data = productToUpdate
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm ID: {Id}", id);
        //        return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, [FromBody] ProductCRUDViewModel model)
        {
            try
            {
                _logger.LogInformation("EditProduct được gọi với ID: {Id}, Model: {@Model}", id, model);

                if (id != model.ProductID)
                {
                    return Json(new { success = false, message = "ID sản phẩm không khớp." });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    _logger.LogWarning("Model không hợp lệ: {Errors}", string.Join(", ", errors));
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ", errors = errors });
                }

                var companyId = await GetCurrentCompanyIdAsync();
                if (companyId == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin doanh nghiệp." });
                }

                var productToUpdate = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductID == id && p.CompanyID == companyId.Value);

                if (productToUpdate == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm hoặc bạn không có quyền sửa." });
                }

                productToUpdate.CategoryID = model.CategoryID;
                productToUpdate.ProductName = model.ProductName;
                productToUpdate.ProductDescription = model.ProductDescription;
                productToUpdate.ProductImage = model.ProductImage;
                productToUpdate.ProductPrice = model.ProductPrice;
                productToUpdate.ProductType = model.ProductType;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Sản phẩm ID {ProductID} được cập nhật thành công", id);

                // Load Category để trả về
                await _context.Entry(productToUpdate).Reference(p => p.Category).LoadAsync();
                return Json(new
                {
                    success = true,
                    message = "Cập nhật sản phẩm thành công",
                    data = new
                    {
                        ProductID = productToUpdate.ProductID,
                        ProductName = productToUpdate.ProductName,
                        ProductPrice = productToUpdate.ProductPrice,
                        ProductType = productToUpdate.ProductType,
                        CategoryID = productToUpdate.CategoryID,
                        // Thêm CategoryName để JS có thể dùng ngay
                        CategoryName = productToUpdate.Category?.CategoryName ?? "N/A"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật sản phẩm ID: {Id}", id);
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                _logger.LogInformation("DeleteProduct được gọi với ID: {Id}", id);

                var companyId = await GetCurrentCompanyIdAsync();
                if (companyId == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin doanh nghiệp." });
                }

                var productToDelete = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductID == id && p.CompanyID == companyId.Value);

                if (productToDelete == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm hoặc bạn không có quyền xóa." });
                }

                _context.Products.Remove(productToDelete);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Sản phẩm ID {ProductID} được xóa thành công", id);

                return Json(new { success = true, message = "Xóa sản phẩm thành công." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm ID: {Id}", id);
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }
    }
}
