

using Microsoft.AspNetCore.Mvc;
using SocialReview.ViewModels;
using SocialReview.Services;
using System.Threading.Tasks; // <-- Thêm thư viện Task
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SocialReview.Models; // <-- Thêm thư viện Authorization (nếu cần cho Logout)

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;

    public AuthController(IAuthService authService,UserManager<User> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }

    // --- Đăng nhập ---
    [HttpGet]
    public IActionResult Login(string returnUrl = "/") // Thêm returnUrl để chuyển hướng sau khi login
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "/")
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var role = await _authService.LoginAsync(model.Username, model.Password);

        if (role == null)
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }

        if (role == "Admin") { 
        return RedirectToAction("Index", "AdminDashboard", new { area = "Admin" });
        }
        else if(role == "Company")
        {
            return LocalRedirect("/CompanyDashboard/Index");
        }
        else
        {
            return LocalRedirect("/Profile/Index");
        }

    }

    // --- Đăng ký ---
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken] 
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.RegisterAsync(model);

        if (result.Succeeded) 
        {
           
            return RedirectToAction("Login");
        }
        else
        {
            //ModelState.AddModelError(string.Empty, );
            //return View(model);
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }

    // --- Đăng xuất ---
    // SỬA LỖI 1: Thêm [HttpPost]
    [HttpPost]
    [ValidateAntiForgeryToken] // <-- Thêm để chống CSRF
    // SỬA LỖI 2: Hàm phải là 'async Task<IActionResult>'
    public async Task<IActionResult> Logout()
    {
        // SỬA LỖI 3: Phải 'await' hàm LogoutAsync()
        await _authService.LogoutAsync();
        // Chuyển về trang chủ sau khi đăng xuất
        return RedirectToAction("Index", "Home");
    }
}

