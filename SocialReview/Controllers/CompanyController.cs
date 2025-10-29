using Microsoft.AspNetCore.Mvc;

namespace SocialReview.Controllers
{
    public class CompanyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
