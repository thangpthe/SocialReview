using Microsoft.AspNetCore.Mvc;

namespace SocialReview.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
