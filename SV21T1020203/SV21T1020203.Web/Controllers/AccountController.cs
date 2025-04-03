using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020203.BusinessLayers;
using SV21T1020203.Web;
using System.Data;
using System.Security.Claims;

namespace SV21T1020203.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            ViewBag.UserName = userName;

            //Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập đầy đủ tên và mật khẩu");
                return View();
            }

            //TODO: Kiểm tra xem userName và password (của Employee) có đúng hay không?
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, userName, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            //Đăng nhập thành công
            WebUserData userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            //2.Ghi nhận trạng thái đăng nhập
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());

            //3. Quay về trang chủ
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        // Xử lý đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var username = User.FindFirstValue(nameof(WebUserData.UserName));

            // Kiểm tra mật khẩu mới và xác nhận mật khẩu
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("Password", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                return View();
            }

            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("", "Không tìm thấy tên người dùng.");
                return View();
            }

            // Xác thực người dùng
            var userType = UserTypes.Employee;
            var userAccount = UserAccountService.Authorize(userType, username, oldPassword);
            if (userAccount == null)
            {
                ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng.");
                return View();
            }

            // Thay đổi mật khẩu
            bool isPasswordChanged = UserAccountService.ChangePassword(userType, username, oldPassword, newPassword);
            if (isPasswordChanged)
            {
                ViewBag.Message = "Đổi mật khẩu thành công!";
                return View();

            }
            else
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi đổi mật khẩu. Vui lòng thử lại.");
                return View();
            }
        }
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public IActionResult AccessDenined()
        {
            return View();
        }
    }
}
