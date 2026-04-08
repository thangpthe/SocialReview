using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialReview.Data;
using SocialReview.Repositories.Interface;

namespace SocialReview.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ApplicationDbContext _context;
        public CompanyController(ICompanyRepository companyRepository, ApplicationDbContext context)
        {
            _companyRepository = companyRepository;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var companies = await _companyRepository.GetAllAsync();
            return View(companies);
        }

        public async Task<IActionResult> Details(int id)
        {
            // Dùng _context để lấy trực tiếp từ Database cho chắc chắn
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null)
            {
                return NotFound("Không tìm thấy doanh nghiệp.");
            }
            return View(company);
        }
    }
}
