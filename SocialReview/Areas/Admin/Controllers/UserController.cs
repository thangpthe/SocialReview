using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialReview.Data;
using SocialReview.Repositories.Interface;

namespace SocialReview.Areas.Admin.Controllers
{
    [Area("Admin")] 
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        public UserController(IUserRepository userRepository, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUser();
            return View(users);
        }
    }
}