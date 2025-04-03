using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020203.BusinessLayers;
using SV21T1020203.DomainModels;
using SV21T1020203.Web.AppCodes;
using SV21T1020203.Web.Models;

namespace SV21T1020203.Web.Controllers
{
  [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR}")]
  public class CustomerController : Controller
  {
    private const int PAGE_SIZE = 20;
    private const string CUSTOMER_SEARCH_CONDITION = "CustomerSearchCondition";


    public IActionResult Index()
    {
      PaginationSearchInput? condition = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH_CONDITION);
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
      var data = CommonDataService.ListOfCustomers(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
      CustomerSearchResult model = new CustomerSearchResult()
      {
        Page = condition.Page,
        PageSize = condition.PageSize,
        SearchValue = condition.SearchValue ?? "",
        RowCount = rowCount,
        Data = data
      };

      ApplicationContext.SetSessionData(CUSTOMER_SEARCH_CONDITION, condition);
      return View(model);
    }

    public IActionResult Create()
    {
      ViewBag.Title = "Bổ sung khách hàng mới";
      var data = new Customer()
      {
        CustomerID = 0,
        IsLocked = false,
      };

      return View("Edit", data);
    }

    public IActionResult Edit(int id = 0)
    {
      ViewBag.Title = "Cập nhật thông tin khách hàng";
      var data = CommonDataService.GetCustomer(id);
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
        CommonDataService.DeleteCustomer(id);
        return RedirectToAction("Index");
      }
      ViewBag.Title = "Xoá thông tin khách hàng";
      var data = CommonDataService.GetCustomer(id);
      if (data == null)
      {
        return RedirectToAction("Index");
      }
      return View(data);
    }

    [HttpPost]
    public IActionResult Save(Customer data)
    {
      ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng mới" : "Cập nhật thông tin khách hàng";
      //Kiểm tra nếu dữ liệu đầu vào không hợp lệ thì tạo ra một thông báo lỗi và lưu trữ vào ModelState
      if (string.IsNullOrWhiteSpace(data.CustomerName))
        ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
      if (string.IsNullOrWhiteSpace(data.ContactName))
        ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
      if (string.IsNullOrWhiteSpace(data.Phone))
        ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập điện thoại của khách hàng");
      if (string.IsNullOrWhiteSpace(data.Email))
        ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email của khách hàng");
      if (string.IsNullOrWhiteSpace(data.Address))
        ModelState.AddModelError(nameof(data.Address), "Vui lòng nhập địa chỉ của khách hàng");
      if (string.IsNullOrEmpty(data.Province))
        ModelState.AddModelError(nameof(data.Province), "Hãy chọn tỉnh/thành cho khách hàng");
      //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay không?
      if (ModelState.IsValid == false)
      {
        return View("Edit", data);
      }
      try
      {
        if (data.CustomerID == 0)
        {
          int id = CommonDataService.AddCustomer(data);
          if (id <= 0)
          {
            ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
            return View("Edit", data);
          }
        }
        else
        {
          bool result = CommonDataService.UpdateCustomer(data);
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
