
using Microsoft.AspNetCore.Mvc;
using SocialReview.ViewModels;
using SocialReview.Services;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(model.Username, model.Password);
        if (!result)
        {
            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await _authService.RegisterAsync(model);
        if (!success)
        {
            ModelState.AddModelError("", "Email đã tồn tại hoặc mật khẩu không hợp lệ.");
            return View(model);
        }

        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        _authService.LogoutAsync();
        return RedirectToAction("Login");
    }
}
