using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020203.BusinessLayers;
using SV21T1020203.DomainModels;
using SV21T1020203.Web.AppCodes;
using SV21T1020203.Web.Models;

namespace SV21T1020203.Web.Controllers
{
  [Authorize(Roles = $"{WebUserRoles.EMPLOYEE}")]

  public class ProductController : Controller
  {
    private const int PAGE_SIZE = 30;
    private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
    public IActionResult Index()
    {
      ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
      if (condition == null)
        condition = new ProductSearchInput()
        {
          Page = 1,
          PageSize = PAGE_SIZE,
          SearchValue = "",
          CategoryID = 0,
          SupplierID = 0,
          MinPrice = 0,
          MaxPrice = 0,

        };
      return View(condition);
    }
    public IActionResult Search(ProductSearchResult condition)
    {
      int rowCount;

      var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "", condition.CategoryID, condition.SupplierID,
                                                  condition.MinPrice, condition.MaxPrice);
      ProductSearchResult model = new ProductSearchResult()
      {
        Page = condition.Page,
        PageSize = condition.PageSize,
        SearchValue = condition.SearchValue ?? "",
        RowCount = rowCount,
        CategoryID = condition.CategoryID,
        SupplierID = condition.SupplierID,
        MinPrice = condition.MinPrice,
        MaxPrice = condition.MaxPrice,
        Data = data
      };
      ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);

      return View(model);
    }
    public IActionResult Create()
    {
      ViewBag.Title = "Bổ sung mặt hàng";
      var data = new Product()
      {
        ProductID = 0,
        IsSelling = false,
        Photo = ""
      };
      return View("Edit", data);
    }
    public IActionResult Edit(int id = 0)
    {
      ViewBag.Title = "Cập nhật thông tin mặt hàng";
      var data = ProductDataService.GetProduct(id);
      if (data == null) return RedirectToAction("Index");
      return View(data);
    }
    public IActionResult Save(Product data, IFormFile? uploadPhoto)
    {
      ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";
      if (string.IsNullOrWhiteSpace(data.ProductName))
        ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được rỗng");
      if (string.IsNullOrWhiteSpace(data.ProductDescription))
        data.ProductDescription = "";
      if (data.CategoryID == 0)
        ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng");
      if (data.SupplierID == 0)
        ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp");
      if (string.IsNullOrWhiteSpace(data.Unit))
        ModelState.AddModelError(nameof(data.Unit), "Vui lòng nhập đơn vị tính");
      if (!int.TryParse(data.Price.ToString(), out var price) || price == 0)
        ModelState.AddModelError(nameof(data.Price), "Vui lòng nhập giá mặt hàng");

      if (!ModelState.IsValid)
      {
        return View("Edit", data);
      }
      // Xử lý ảnh
      if (uploadPhoto != null)
      {
        string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; //Tên file sẽ lưu

        string filePath = Path.Combine(ApplicationContext.WebRootPath, "images/products", fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          uploadPhoto.CopyTo(stream);
        }
        data.Photo = fileName;
      }
      if (data.ProductID == 0)
      {
        int result = ProductDataService.AddProduct(data);
        if (result > 0)
        {
          return RedirectToAction("Edit", new { id = result });
        }

      }
      else
      {
        bool result = ProductDataService.UpdateProduct(data);
      }
      return RedirectToAction("Index");
    }
    public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
    {
      ViewBag.Title = data.PhotoID == 0 ? "Bổ sung thư viện ảnh" : "Cập nhật thông tin ảnh";
      if (uploadPhoto == null)
        ModelState.AddModelError(nameof(data.Photo), "Vui lòng chọn ảnh");

      // Chi kiem tra khong duoc rong khi create
      if (data.PhotoID == 0)
      {
        if (!ModelState.IsValid)
        {
          return View("Photo", data);
        }
      }
      // Xử lý ảnh
      if (uploadPhoto != null)
      {
        string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; //Tên file sẽ lưu

        string filePath = Path.Combine(ApplicationContext.WebRootPath, "images/products", fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
          uploadPhoto.CopyTo(stream);
        }
        data.Photo = fileName;
      }
      if (data.PhotoID == 0)
      {
        ProductDataService.AddPhoto(data);
      }
      else
      {
        ProductDataService.UpdatePhoto(data);
      }
      return RedirectToAction("Edit", new { id = data.ProductID });
    }
    public IActionResult SaveAttribute(ProductAttribute data)
    {
      ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính" : "Cập nhật thông tin thuộc tính";
      if (string.IsNullOrWhiteSpace(data.AttributeName))
        ModelState.AddModelError(nameof(data.AttributeName), "Vui lòng nhập tên thuộc tính");
      if (string.IsNullOrWhiteSpace(data.AttributeValue))
        ModelState.AddModelError(nameof(data.AttributeValue), "Vui lòng nhập giá trị thuộc tính");
      if (!ModelState.IsValid)
      {
        return View("Attribute", data);
      }

      if (data.AttributeID == 0)
      {
        ProductDataService.AddAttribute(data);
      }
      else
      {
        ProductDataService.UpdateAttribute(data);
      }
      return RedirectToAction("Edit", new { id = data.ProductID });

    }
    public IActionResult Delete(int id = 0)
    {
      if (Request.Method == "POST")
      {
        ProductDataService.DeleteProduct(id);
        return RedirectToAction("Index");
      }
      var data = ProductDataService.GetProduct(id);
      if (data == null) return RedirectToAction("Index");
      return View(data);
    }
    public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
    {
      switch (method)
      {
        case "add":
          ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
          var data = new ProductPhoto()
          {
            PhotoID = 0,
            ProductID = id,
            Photo = "",
            IsHidden = false,

          };
          return View(data);
        case "edit":
          ViewBag.Title = "Thay đổi ảnh của mặt hàng";
          var data1 = ProductDataService.GetPhoto(photoId);
          if (data1 == null) return RedirectToAction("Index");
          return View(data1);
        case "delete":
          //TODO: Xóa ảnh(xóa trực tiếp, không cần confirm)
          ProductDataService.DeletePhoto(photoId);
          return RedirectToAction("Edit", new { id = id });
        default:
          return RedirectToAction("Index");
      }
    }
    public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
    {
      switch (method)
      {
        case "add":
          ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
          var data = new ProductAttribute()
          {
            AttributeID = 0,
            ProductID = id,
          };
          return View(data);
        case "edit":
          ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
          var data1 = ProductDataService.GetAttribute(attributeId);
          return View(data1);
        case "delete":
          //TODO: Xóa ảnh(xóa trực tiếp, không cần confirm)
          ProductDataService.DeleteAttribute(attributeId);
          return RedirectToAction("Edit", new { id = id });
        default:
          return RedirectToAction("Index");
      }
    }

  }
}
