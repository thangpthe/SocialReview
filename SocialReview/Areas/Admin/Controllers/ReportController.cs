using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SocialReview.Data;
using SocialReview.Models;
using SocialReview.ViewModels;
using System.IO;

namespace SocialReview.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new ReportViewModel
            {
                TotalUsers = _context.Users.Count(),
                TotalCompanies = _context.Companies.Count(),
                TotalProducts = _context.Products.Count(),
                TotalReviews = _context.Reviews.Count(),
                TotalComments = _context.Comments.Count() // giả sử bạn có DbSet<Comment>
            };
            return View(model);
        }

        // ====================== EXPORT EXCEL ======================
        public IActionResult ExportCompanies()
        {
            var companies = _context.Companies.ToList();
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Doanh nghiệp");

            ws.Cells[1, 1].Value = "ID";
            ws.Cells[1, 2].Value = "Tên doanh nghiệp";
            ws.Cells[1, 3].Value = "Mô tả";
            ws.Cells[1, 4].Value = "Lĩnh vực";
            ws.Cells[1, 5].Value = "Email";
            ws.Cells[1, 6].Value = "SĐT";

            for (int i = 0; i < companies.Count; i++)
            {
                var c = companies[i];
                ws.Cells[i + 2, 1].Value = c.CompanyID;
                ws.Cells[i + 2, 2].Value = c.CompanyName;
                ws.Cells[i + 2, 3].Value = c.CompanyDescription;
                ws.Cells[i + 2, 4].Value = c.Industry;
                ws.Cells[i + 2, 5].Value = c.ContactEmail;
                ws.Cells[i + 2, 6].Value = c.Phone;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            var fileBytes = package.GetAsByteArray();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCao_DoanhNghiep.xlsx");
        }

        public IActionResult ExportProducts()
        {
            var products = _context.Products.ToList();
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("SanPham");

            ws.Cells[1, 1].Value = "ID";
            ws.Cells[1, 2].Value = "Tên sản phẩm";
            ws.Cells[1, 3].Value = "Mô tả";
            ws.Cells[1, 4].Value = "Giá";
            ws.Cells[1, 5].Value = "Loại";
            ws.Cells[1, 6].Value = "Trạng thái";

            for (int i = 0; i < products.Count; i++)
            {
                var p = products[i];
                ws.Cells[i + 2, 1].Value = p.ProductID;
                ws.Cells[i + 2, 2].Value = p.ProductName;
                ws.Cells[i + 2, 3].Value = p.ProductDescription;
                ws.Cells[i + 2, 4].Value = p.ProductPrice;
                ws.Cells[i + 2, 5].Value = p.ProductType;
                ws.Cells[i + 2, 6].Value = p.Disabled == true ? "Đã ẩn" : "Hoạt động";
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            var fileBytes = package.GetAsByteArray();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCao_SanPham.xlsx");
        }

        // Bạn có thể copy-paste tương tự cho ExportUsers, ExportReviews, ExportComments

        public IActionResult ExportUsers()
        {
            var users = _context.Users.ToList();
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("NguoiDung");

            ws.Cells[1, 1].Value = "ID";
            ws.Cells[1, 2].Value = "Username";
            ws.Cells[1, 3].Value = "Vai trò";
            ws.Cells[1, 4].Value = "Email";

            for (int i = 0; i < users.Count; i++)
            {
                var u = users[i];
                ws.Cells[i + 2, 1].Value = u.Id;
                ws.Cells[i + 2, 2].Value = u.UserName;
                ws.Cells[i + 2, 3].Value = u.UserRole;
                ws.Cells[i + 2, 4].Value = u.Email;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCao_NguoiDung.xlsx");
        }

        // Thêm ExportReviews và ExportComments tương tự nếu cần
    }
}