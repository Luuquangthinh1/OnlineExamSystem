using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Data;
using OnlineExamSystem.Models;
public class AuthController : Controller
{
    private readonly OnlineExamSystemContext _context;

    public AuthController(OnlineExamSystemContext context)
    {
        _context = context;
    }
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) &&
            string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Vui lòng nhập tên tài khoản và mật khẩu";
            return View();
        }
        if (string.IsNullOrWhiteSpace(username))
        {
            ViewBag.Error = "Vui lòng nhập tên tài khoản";
            return View();
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Vui lòng nhập mật khẩu";
            return View();
        }
        var user = _context.Users.FirstOrDefault(x =>
            x.Username.Trim().ToLower() == username.Trim().ToLower() &&
            x.Password.Trim() == password.Trim()
        );
        if (user == null)
        {
            ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
            return View();
        }
        HttpContext.Session.SetInt32(
     "UserId",
     user.Id);

        HttpContext.Session.SetString(
            "User",
            user.Username);

        HttpContext.Session.SetString(
            "Role",
            user.Role.ToString());
        user.LastLoginAt = DateTime.Now;

        user.LoginCount += 1;

        _context.Users.Update(user);

        _context.SaveChanges();
        if (user.Role == Role.Teacher)
        {
            return RedirectToAction("Index", "Teacher");
        }
        else if (user.Role == Role.Admin)
        {
            return RedirectToAction("Users", "Admin");
        }
        return RedirectToAction("Index", "Student");
    }
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Register(
    string username,
    string password,
    string role)
    {
        if (string.IsNullOrWhiteSpace(username) &&
            string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error =
                "Vui lòng nhập tên tài khoản và mật khẩu";

            return View();
        }
        if (string.IsNullOrWhiteSpace(username))
        {
            ViewBag.Error =
                "Vui lòng nhập tên tài khoản";

            return View();
        }
        if (string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error =
                "Vui lòng nhập mật khẩu";

            return View();
        }
        if (_context.Users.Any(x =>
            x.Username.Trim().ToLower() ==
            username.Trim().ToLower()))
        {
            ViewBag.Error =
                "Tài khoản đã tồn tại";

            return View();
        }
        if (!Enum.TryParse<Role>(
                role,
                true,
                out var roleEnum))
        {
            roleEnum = Role.Student;
        }
        var user = new User
        {
            Username = username,
            Password = password,
            Role = roleEnum
        };
        _context.Users.Add(user);
        _context.SaveChanges();
        return RedirectToAction("Login");
    }
    public IActionResult ForgotPassword()
    {
        return View();
    }
    [HttpPost]
    public IActionResult ForgotPassword(
    string? username,
    string? newPassword,
    string? confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            ViewBag.Error = "Vui lòng nhập tên đăng nhập";
            return View();
        }

        if (string.IsNullOrWhiteSpace(newPassword))
        {
            ViewBag.Error = "Vui lòng nhập mật khẩu mới";
            return View();
        }

        if (newPassword != confirmPassword)
        {
            ViewBag.Error = "Mật khẩu xác nhận không khớp";
            return View();
        }

        var normalizedUsername = username.Trim();

        var user = _context.Users
            .FirstOrDefault(x => x.Username != null &&
                                 x.Username == normalizedUsername);

        if (user == null)
        {
            ViewBag.Error = "Không tìm thấy tài khoản";
            return View();
        }

        user.Password = newPassword;
        _context.SaveChanges();

        ViewBag.Success = "Đổi mật khẩu thành công";
        return View();
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}