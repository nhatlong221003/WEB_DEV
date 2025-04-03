
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020203.BusinessLayers;
using SV21T1020203.DomainModels;
using SV21T1020203.Web.AppCodes;
using SV21T1020203.Web.Models;
namespace SV21T1020203.Web.Controllers
{
  [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR}")]
  public class EmployeeController : Controller
  {
    public const int PAGE_SIZE = 9;
    private const string EMPLOYEE_SEARCH_CONDITION = "EmployeeSearchCondition";
    public IActionResult Index()
    {
      PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH_CONDITION);
      if (condition == null)
        condition = new PaginationSearchInput()
        {
          Page = 1,
          PageSize = PAGE_SIZE,
          SearchValue = ""
        };
      return View(condition);
    }

    public IActionResult Search(PaginationSearchInput condition)
    {
      int rowCount;
      var data = CommonDataService.ListOfEmployees(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");

      EmployeeSearchResult model = new EmployeeSearchResult()
      {
        Page = condition.Page,
        PageSize = condition.PageSize,
        SearchValue = condition.SearchValue ?? "",
        RowCount = rowCount,
        Data = data
      };
      ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_CONDITION, condition);
      return View(model);
    }

    public IActionResult Create()
    {
      ViewBag.Title = "Bổ sung thông tin nhân viên";
      var data = new Employee()
      {
        EmployeeID = 0,
        IsWorking = true,
        Photo = "nophoto.png"
      };
      return View("Edit", data);
    }
    public IActionResult Edit(int id = 0)
    {
      ViewBag.Title = "Chỉnh sửa thông tin nhân viên";
      var data = CommonDataService.GetEmployee(id);
      if (data == null)
      {
        return RedirectToAction("Index");
      }
      return View(data);

    }
    public IActionResult Delete(int id = 0)
    {
      if (Request.Method == "POST")
      {
        CommonDataService.DeleteEmployee(id);
        return RedirectToAction("Index");
      }
      ViewBag.Title = "Xoá thông tin nhân viên";
      var data = CommonDataService.GetEmployee(id);
      if (data == null)
      {
        return RedirectToAction("Index");
      }
      return View(data);
    }

    [HttpPost]
    public IActionResult Save(Employee data, string _BirthDate, IFormFile? _Photo)
    {
      ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên mới" : "Cập nhật thông tin nhân viên";
      //Kiểm tra nếu dữ liệu đầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào ModelState
      if (string.IsNullOrWhiteSpace(data.FullName))
        ModelState.AddModelError(nameof(data.FullName), "Tên nhân viên không được để trống");
      if (string.IsNullOrEmpty(_BirthDate))
        ModelState.AddModelError(nameof(data.BirthDate), "Vui lòng nhập ngày sinh cho nhân viên");
      if (string.IsNullOrWhiteSpace(data.Phone))
        ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập điện thoại của nhân viên");
      if (string.IsNullOrWhiteSpace(data.Email))
        ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của nhân viên");
      if (string.IsNullOrWhiteSpace(data.Address))
        ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của nhân viên");

      //Xử lí cho ngày sinh
      DateTime? d = _BirthDate.ToDateTime();
      if (d.HasValue && d >= new DateTime(1753, 1, 1))
        data.BirthDate = d.Value;
      else
      {
        ModelState.AddModelError(nameof(data.BirthDate), "Ngày sinh của nhân viên không hợp lệ");
        return View("Edit", data);
      }
      //Xử lí với ảnh
      if (_Photo != null)
      {
        string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
        string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images/employees", fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          _Photo.CopyTo(stream);
        }
        data.Photo = fileName;
      }
      //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay không?
      if (ModelState.IsValid == false)
      {
        return View("Edit", data);
      }

      try
      {
        if (data.EmployeeID == 0)
        {
          int id = CommonDataService.AddEmployee(data);
          if (id <= 0)
          {
            ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
            return View("Edit", data);
          }
        }
        else
        {
          bool result = CommonDataService.UpdateEmployee(data);
          if (result == false)
          {
            ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
            return View("Edit", data);
          }
        }
        return RedirectToAction("Index");

      }
      catch
      {
        ModelState.AddModelError("Error", "Hệ thống tạm thời gián đoạn");
        return View("Edit");
      }
    }
  }
}
