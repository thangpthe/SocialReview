using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialReview.Data;
using SocialReview.Repositories.Interface;

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
    }
}
