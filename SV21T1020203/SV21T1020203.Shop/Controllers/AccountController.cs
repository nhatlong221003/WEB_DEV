using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020203.BusinessLayers;
using SV21T1020203.DomainModels;
using SV21T1020203.Shop;

using System.Data;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020203.Shop.Controllers
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

      //TODO: Kiểm tra xem userName và password (của Customer) có đúng hay không?
      var userAccount = UserAccountService.Authorize(UserTypes.Customer, userName, password);
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
      return RedirectToAction("Index", "Product");
    }

    // Xử lí đăng xuất
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
      var userType = UserTypes.Customer;
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

    // Xử lí thay đổi thông tin cá nhân
    [HttpGet]
    public IActionResult EditProfile()
    {
      ViewBag.Title = "Cập nhật thông tin cá nhân";

      var userId = User.FindFirstValue("UserId");
      if (string.IsNullOrEmpty(userId))
      {
        return RedirectToAction("Login", "Account");
      }

      var userData = CommonDataService.GetCustomer(int.Parse(userId));
      if (userData == null)
      {
        return RedirectToAction("Index", "Product");
      }

      return View(userData);
    }

    [HttpPost]
    public IActionResult EditProfile(Customer model)
    {
      ViewBag.Title = "Cập nhật thông tin cá nhân";

      if (string.IsNullOrWhiteSpace(model.CustomerName))
        ModelState.AddModelError(nameof(model.CustomerName), "Tên khách hàng không được để trống");

      if (string.IsNullOrWhiteSpace(model.ContactName))
        ModelState.AddModelError(nameof(model.ContactName), "Tên giao dịch không được để trống");

      if (string.IsNullOrWhiteSpace(model.Phone))
        ModelState.AddModelError(nameof(model.Phone), "Số điện thoại không được để trống");

      if (ModelState.IsValid == false)
      {
        return View(model);
      }

      try
      {
        bool result = CommonDataService.UpdateCustomer(model);
        if (result == false)
        {
          ModelState.AddModelError(nameof(model.Email), "Email bị trùng");
          return View("EditProfile", model);
        }

        TempData["Message"] = "Cập nhật thông tin thành công!";
        return RedirectToAction("Index", "Product");
      }
      catch
      {
        ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn.");
        return View(model);
      }
    }

    // Đăng ký tài khoản
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
      return View();
    }

    // Xử lý đăng ký tài khoản
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Account/Register")]
    public IActionResult Register(Customer model, string userName, string password, string confirmPassword)
    {
      // Kiểm tra các trường thông tin đăng ký
      if (string.IsNullOrWhiteSpace(userName))
      {
        ModelState.AddModelError(nameof(userName), "Tên đăng nhập không được để trống.");
      }
      if (string.IsNullOrWhiteSpace(password))
      {
        ModelState.AddModelError(nameof(password), "Mật khẩu không được để trống.");
      }
      if (password != confirmPassword)
      {
        ModelState.AddModelError(nameof(confirmPassword), "Mật khẩu xác nhận không khớp.");
      }

      // Kiểm tra các trường Customer
      if (string.IsNullOrWhiteSpace(model.CustomerName))
      {
        ModelState.AddModelError(nameof(model.CustomerName), "Tên khách hàng không được để trống.");
      }
      if (string.IsNullOrWhiteSpace(model.ContactName))
      {
        ModelState.AddModelError(nameof(model.ContactName), "Tên liên hệ không được để trống.");
      }
      if (string.IsNullOrWhiteSpace(model.Address))
      {
        ModelState.AddModelError(nameof(model.Address), "Địa chỉ không được để trống.");
      }
      if (string.IsNullOrWhiteSpace(model.Phone))
      {
        ModelState.AddModelError(nameof(model.Phone), "Số điện thoại không được để trống.");
      }

      // Nếu có lỗi thì trả về view với thông tin lỗi
      if (!ModelState.IsValid)
      {
        return View(model);
      }

      // Kiểm tra email có bị trùng hay không
      var existingCustomer = CommonDataService.ListOfCustomers().FirstOrDefault(c => c.Email == userName);
      if (existingCustomer != null)
      {
        ModelState.AddModelError(nameof(model.Email), "Email đã tồn tại.");
        return View(model);
      }

      // Kiểm tra xem tài khoản đã tồn tại chưa
      var existingUser = UserAccountService.Authorize(UserTypes.Customer, userName, password);
      if (existingUser != null)
      {
        ModelState.AddModelError(nameof(userName), "Tài khoản đã tồn tại.");
        return View(model);
      }

      // Tạo mới tài khoản
      bool isRegistered = UserAccountService.Register(model, userName, password);
      if (isRegistered)
      {
        TempData["Message"] = "Đăng ký thành công! Vui lòng đăng nhập.";
        return RedirectToAction("Login");
      }
      else
      {
        ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại.");
        return View(model);
      }
    }


  }
}
