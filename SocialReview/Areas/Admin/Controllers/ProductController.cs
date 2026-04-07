using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialReview.Data;
using SocialReview.Migrations;
using SocialReview.Models;
using SocialReview.Repositories.Interface;
using SocialReview.ViewModels;

namespace SocialReview.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        public ProductController(IProductRepository productRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var companies = await _productRepository.GetAllAsync();
            return View(companies);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ToggleStatus(int id)
        //{
        //    // Vì IProductRepository chưa rõ có GetByIdAsync hay UpdateAsync không,
        //    // chúng ta sẽ dùng _context trực tiếp cho an toàn.
        //    var product = await _context.Products.FindAsync(id);

        //    if (product != null)
        //    {
        //        // Lật ngược trạng thái: true thành false, false thành true
        //        product.Disabled = !product.Disabled;

        //        _context.Update(product);
        //        await _context.SaveChangesAsync();

        //        TempData["SuccessMessage"] = "Cập nhật trạng thái sản phẩm thành công!";
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
        //    }

        //    return RedirectToAction(nameof(Index)); // Quay lại trang danh sách
        //}

        [HttpPost]
        [IgnoreAntiforgeryToken] // Bỏ qua kiểm tra Antiforgery cho AJAX
        // Định tuyến rõ ràng để JavaScript dễ dàng gọi
        [Route("Admin/Product/ToggleStatus/{id:int}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                // Trả về lỗi dạng JSON
                return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
            }

            try
            {
                // Lật ngược trạng thái: true thành false, false thành true
                product.Disabled = !product.Disabled;

                _context.Update(product);
                await _context.SaveChangesAsync();

                // Trả về kết quả thành công dạng JSON
                return Json(new
                {
                    success = true,
                    message = "Cập nhật trạng thái thành công!",
                    newIsDisabledState = product.Disabled
                });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (ex) nếu cần
                return Json(new { success = false, message = "Lỗi khi cập nhật cơ sở dữ liệu." });
            }
        }

    }
}
