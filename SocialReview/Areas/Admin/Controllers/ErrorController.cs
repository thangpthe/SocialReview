using Microsoft.AspNetCore.Mvc;

namespace SocialReview.Areas.Admin.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult StatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return View("NotFound");
                case 403:
                    return View("AccessDenied");
                default:
                    
                    return View("NotFound");
            }
        }
    }
}
