using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020203.BusinessLayers;
using SV21T1020203.DomainModels;
using SV21T1020203.Shop.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SV21T1020203.Shop.Controllers
{
  public class ProductController : Controller
  {
    private const int PAGE_SIZE = 30;
    private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";

    public IActionResult Index()
    {
      // Retrieve search condition from session
      ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
      if (condition == null)
      {
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
      }
      return View(condition);
    }

    public IActionResult Search(ProductSearchResult condition)
    {
      int rowCount;

      // Retrieve products based on search criteria
      var data = ProductDataService.ListProducts(
          out rowCount,
          condition.Page,
          condition.PageSize,
          condition.SearchValue ?? "",
          condition.CategoryID,
          condition.SupplierID,
          condition.MinPrice,
          condition.MaxPrice) ?? new List<Product>(); // Fallback to empty list

      // Create model for the view
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

      // Store the search condition in session
      ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);

      return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
      // Kiểm tra tính hợp lệ của ID sản phẩm
      if (id <= 0)
      {
        return BadRequest("ID sản phẩm không hợp lệ.");
      }

      // Lấy thông tin chi tiết sản phẩm
      var product = await ProductDataService.GetProductByIdAsync(id);
      if (product == null)
      {
        return NotFound(); // Trả về 404 nếu không tìm thấy sản phẩm
      }

      // Trả về view với mô hình sản phẩm
      return View(product); // Đảm bảo view này được thiết lập để chấp nhận SV21T1020203.DomainModels.Product
    }
  }
}