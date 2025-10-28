using Microsoft.AspNetCore.Mvc;

namespace SocialReview.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
