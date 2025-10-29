using Microsoft.AspNetCore.Mvc;

namespace SocialReview.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
