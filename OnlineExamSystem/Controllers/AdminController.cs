using Microsoft.AspNetCore.Mvc;
using OnlineExamSystem.Data;
using OnlineExamSystem.Models;

namespace OnlineExamSystem.Controllers
{
    public class AdminController
        : Controller
    {
        private readonly
            OnlineExamSystemContext
            _context;

        public AdminController(
            OnlineExamSystemContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            var role =
                HttpContext.Session
                    .GetString("Role");

            return role == "Admin";
        }
        public IActionResult Dashboard()
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            ViewBag.TotalUsers =
                _context.Users.Count();

            ViewBag.TotalStudents =
                _context.Users.Count(x =>
                    x.Role == Role.Student);

            ViewBag.TotalTeachers =
                _context.Users.Count(x =>
                    x.Role == Role.Teacher);

            ViewBag.TotalAdmins =
                _context.Users.Count(x =>
                    x.Role == Role.Admin);

            ViewBag.TotalLogins =
                _context.Users.Sum(x =>
                    x.LoginCount);

            ViewBag.ActiveToday =
                _context.Users.Count(x =>
                    x.LastLoginAt != null &&
                    x.LastLoginAt.Value.Date
                        == DateTime.Today);

            var latestUsers =
                _context.Users
                    .OrderByDescending(x =>
                        x.LastLoginAt)
                    .Take(10)
                    .ToList();

            return View(latestUsers);
        }
        public IActionResult Users()
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            var users =
                _context.Users.ToList();

            return View(users);
        }
        public IActionResult UserDetails(
            int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            var user =
                _context.Users
                    .FirstOrDefault(x =>
                        x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        public IActionResult EditUser(
            int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            var user =
                _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(
            User user,
            string role)
        {
            var oldUser =
                _context.Users
                    .FirstOrDefault(x =>
                        x.Id == user.Id);

            if (oldUser == null)
            {
                return NotFound();
            }

            if (!Enum.TryParse<Role>(
                role,
                out var roleEnum))
            {
                roleEnum = Role.Student;
            }

            oldUser.FullName =
                user.FullName?.Trim() ?? "";

            oldUser.Username =
                user.Username?.Trim() ?? "";

            oldUser.Email =
                user.Email?.Trim() ?? "";

            oldUser.PhoneNumber =
                user.PhoneNumber?.Trim() ?? "";

            oldUser.Role =
                roleEnum;

            _context.SaveChanges();

            TempData["Success"] =
                "Cập nhật thành công";

            return RedirectToAction(
                "Users");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(
            int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction(
                    "Login",
                    "Auth");
            }

            var user =
                _context.Users.Find(id);

            if (user != null)
            {
                _context.Users.Remove(user);

                _context.SaveChanges();
            }

            return RedirectToAction(
                "Users");
        }
    }
}