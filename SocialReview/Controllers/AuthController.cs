
//using Microsoft.AspNetCore.Mvc;
//using SocialReview.ViewModels;
//using SocialReview.Services;

//public class AuthController : Controller
//{
//    private readonly IAuthService _authService;

//    public AuthController(IAuthService authService)
//    {
//        _authService = authService;
//    }

//    [HttpGet]
//    public IActionResult Login() => View();

//    [HttpPost]
//    public async Task<IActionResult> Login(LoginViewModel model)
//    {
//        if (!ModelState.IsValid)
//            return View(model);

//        var result = await _authService.LoginAsync(model.Username, model.Password);
//        if (!result)
//        {
//            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
//            return View(model);
//        }

//        return RedirectToAction("Index", "Home");
//    }

//    [HttpGet]
//    public IActionResult Register() => View();

//    [HttpPost]
//    public async Task<IActionResult> Register(RegisterViewModel model)
//    {
//        if (!ModelState.IsValid)
//            return View(model);

//        var success = await _authService.RegisterAsync(model);
//        if (!success)
//        {
//            ModelState.AddModelError("", "Email đã tồn tại hoặc mật khẩu không hợp lệ.");
//            return View(model);
//        }

//        return RedirectToAction("Login");
//    }

//    public IActionResult Logout()
//    {
//        _authService.LogoutAsync();
//        return RedirectToAction("Login");
//    }
//}

using Microsoft.AspNetCore.Mvc;
using SocialReview.ViewModels;
using SocialReview.Services;
using System.Threading.Tasks; // <-- Thêm thư viện Task
using Microsoft.AspNetCore.Authorization; // <-- Thêm thư viện Authorization (nếu cần cho Logout)

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // --- Đăng nhập ---
    [HttpGet]
    public IActionResult Login(string returnUrl = "/") // Thêm returnUrl để chuyển hướng sau khi login
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // <-- Thêm để chống CSRF
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "/")
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.LoginAsync(model.Username, model.Password);
        if (!result)
        {
            // Giữ thông báo lỗi chung chung để bảo mật
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }

        if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/")
        {
            // Nếu đúng, luôn chuyển hướng về trang chủ /Home/Index
            return RedirectToAction("Index", "Home");
        }
        else
        {
            // 2. Nếu returnUrl CÓ giá trị (ví dụ: /Profile/MyPage)
            //    chuyển hướng an toàn về trang đó.
            return LocalRedirect(returnUrl);
        }

    }

    // --- Đăng ký ---
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken] // <-- Thêm để chống CSRF
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // --- Cải thiện xử lý lỗi ---
        var result = await _authService.RegisterAsync(model);

        if (result.Succeeded) // Nếu hàm trả về bool như code của bạn
        {
           
            return RedirectToAction("Login");
        }
        else
        {
            //ModelState.AddModelError(string.Empty, );
            //return View(model);
            foreach (var error in result.Errors)
            {
                // error.Description sẽ là: "Passwords must be at least 6 characters.", "Username 'abc' is already taken."...
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

