using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020203.BusinessLayers;
using SV21T1020203.DomainModels;
using SV21T1020203.Shop;
using SV21T1020203.Shop.AppCodes;
using SV21T1020203.Shop.Models;
using System.Globalization;
using System.Security.Claims;
namespace SV21T1020203.Shop.Controllers
{
  [Authorize]
  public class OrderController : Controller
  {
    public const int PAGE_SIZE = 30;
    private const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";
    private const string SHOPPING_CART = "ShoppingCart";
    private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale";
    private const int PRODUCT_PAGE_SIZE = 5;
    private List<CartItem> GetShoppingCart()
    {
      var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
      if (shoppingCart == null)
      {

        shoppingCart = new List<CartItem>();
        ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
      }
      return shoppingCart;
    }
    public IActionResult Index()
    {

      var condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
      if (condition == null)
      {
        var cultureInfo = new CultureInfo("en-GB");
        condition = new OrderSearchInput()
        {
          Page = 1,
          PageSize = PAGE_SIZE,
          SearchValue = "",
          Status = 0,
          TimeRange = $"{DateTime.Today.AddDays(-720).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy")}"

        };
      }
      return View(condition);
    }

    public IActionResult Search(OrderSearchInput condition)
    {
      int rowCount;
      var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize, condition.Status, condition.FromTime, condition.ToTime, condition.SearchValue ?? "");
      var model = new OrderSearchResult()
      {
        Page = condition.Page,
        PageSize = condition.PageSize,
        SearchValue = condition.SearchValue ?? "",
        Status = condition.Status,
        TimeRange = condition.TimeRange,
        RowCount = rowCount,
        Data = data
      };
      ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);
      return View(model);
    }
    public IActionResult Details(int id = 0)
    {
      var order = OrderDataService.GetOrder(id);
      if (order == null)
      {
        return RedirectToAction("Index");
      }
      var details = OrderDataService.ListOrderDetails(id);
      var model = new OrderDetailModel()
      {
        Order = order,
        Details = details
      };
      return View(model);
    }
    public IActionResult AddToCart(CartItem item)
    {
      if (item.SalePrice < 0 || item.Quantity <= 0)
        return Json("Giá bán và số lượng không hợp lệ");

      var shoppingCart = GetShoppingCart();
      var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);

      if (existsProduct == null)
      {
        shoppingCart.Add(item); // Thêm sản phẩm mới
      }
      else
      {
        existsProduct.Quantity += item.Quantity; // Cập nhật số lượng
        existsProduct.SalePrice = item.SalePrice; // Cập nhật giá
      }

      ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart); // Lưu vào session

      // Sử dụng TempData để lưu thông báo thành công
      TempData["SuccessMessage"] = "Đặt hàng thành công!";

      return Redirect(Request.Headers["Referer"].ToString()); // Chuyển hướng trở lại trang trước
    }


    public IActionResult RemoveFromCart(int id = 0)
    {
      var shoppingCart = GetShoppingCart();
      int index = shoppingCart.FindIndex(m => m.ProductID == id);
      if (index >= 0)
      {
        shoppingCart.RemoveAt(index);
      }
      ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
      return RedirectToAction("Create");

    }

    public IActionResult ClearCart()
    {
      var shoppingCart = GetShoppingCart();
      shoppingCart.Clear();
      ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
      return RedirectToAction("Create");

    }

    public IActionResult ShoppingCart()
    {
      return View(GetShoppingCart());
    }
    public IActionResult Create()
    {
      var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
      if (condition == null)
      {
        condition = new ProductSearchInput()
        {
          Page = 1,
          PageSize = PRODUCT_PAGE_SIZE,
          SearchValue = ""
        };
      }
      return View(condition);
    }
    public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
    {
      try
      {
        var shoppingCart = GetShoppingCart();
        if (shoppingCart.Count == 0)
          return Json("Giỏ hàng trống. Vui lòng chọn mặt hàng cần bán");
        if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
          return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");
        var userData = User.GetUserData();
        // lấy random nhân viên phụ trách
        Random random = new Random();
        int employeeID = random.Next(1, 99);

        List<OrderDetail> orderDetails = new List<OrderDetail>();
        foreach (var item in shoppingCart)
        {
          orderDetails.Add(new OrderDetail()
          {
            ProductID = item.ProductID,
            Quantity = item.Quantity,
            SalePrice = item.SalePrice

          });
        }
        int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
        ClearCart();
        return Json(orderID);
      }
      catch (Exception ex)
      {
        // Ghi log lỗi để kiểm tra
        Console.WriteLine(ex.Message);
        return StatusCode(500, "Đã xảy ra lỗi trong quá trình xử lý đơn hàng.");
      }
    }

    [HttpPost]
    public IActionResult UpdateCart(int ProductID, int Quantity)
    {
      if (Quantity <= 0)
      {
        return Json("Số lượng không hợp lệ.");
      }

      var shoppingCart = GetShoppingCart();
      var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == ProductID);
      if (existsProduct != null)
      {
        existsProduct.Quantity = Quantity;
        ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
      }

      return RedirectToAction("Create");
    }


    public IActionResult Cancel(int id)
    {
      OrderDataService.CancelOrder(id);
      return RedirectToAction("Details", new { id = id });
    }
  }
}
