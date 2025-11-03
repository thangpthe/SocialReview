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

            public CompanyDashboardController(UserManager<User> userManager, ApplicationDbContext context)
            {
                _userManager = userManager;
                _context = context;
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
        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductCRUDViewModel model)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null)
            {
                return Forbid("Không tìm thấy thông tin doanh nghiệp.");
            }

            // Gán CompanyID của user vào model trước khi kiểm tra
            // (Mặc dù chúng ta sẽ gán lại, nhưng validation có thể cần)
            if (!ModelState.IsValid)
            {
                // Trả về lỗi validation
                return BadRequest(ModelState);
            }

            // Map từ ViewModel sang Model CSDL
            var newProduct = new Product
            {
                CompanyID = companyId.Value, // Gán CompanyID của user
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

            // Trả về sản phẩm đã tạo (để JS thêm vào bảng)
            return Ok(newProduct);
        }

        //
        // GET: /CompanyDashboard/GetProductDetails/5
        // (API để lấy thông tin sản phẩm cho form Sửa)
        //
        [HttpGet]
        public async Task<IActionResult> GetProductDetails(int id)
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

            return Ok(viewModel); // Trả về JSON
        }

        //
        // POST: /CompanyDashboard/EditProduct/5
        //
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, [FromBody] ProductCRUDViewModel model)
        {
            if (id != model.ProductID)
            {
                return BadRequest("ID sản phẩm không khớp.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            // Kiểm tra sở hữu
            var productToUpdate = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductID == id && p.CompanyID == companyId.Value);

            if (productToUpdate == null)
            {
                return NotFound("Không tìm thấy sản phẩm hoặc bạn không có quyền sửa.");
            }

            // Cập nhật
            productToUpdate.CategoryID = model.CategoryID;
            productToUpdate.ProductName = model.ProductName;
            productToUpdate.ProductDescription = model.ProductDescription;
            productToUpdate.ProductImage = model.ProductImage;
            productToUpdate.ProductPrice = model.ProductPrice;
            productToUpdate.ProductType = model.ProductType;

            await _context.SaveChangesAsync();

            // Tải lại Category để trả về tên
            await _context.Entry(productToUpdate).Reference(p => p.Category).LoadAsync();

            return Ok(productToUpdate); // Trả về sản phẩm đã cập nhật
        }


        //
        // DELETE: /CompanyDashboard/DeleteProduct/5
        
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var companyId = await GetCurrentCompanyIdAsync();
            if (companyId == null) return Forbid();

            // Kiểm tra sở hữu
            var productToDelete = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductID == id && p.CompanyID == companyId.Value);

            if (productToDelete == null)
            {
                return NotFound("Không tìm thấy sản phẩm hoặc bạn không có quyền xóa.");
            }

            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa sản phẩm thành công." }); // Trả về JSON xác nhận
        }
    }
    }
