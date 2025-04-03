namespace SV21T1020203.Shop.Models
{
  /// <summary>
  /// Lưu giữ các thông tin đầu vào sử dụng cho chức năng tìm kiếm và hiển thị dữ liệu dưới dạng phân trang
  /// </summary>
  public class PaginationSearchInput
  {
    /// <summary>
    /// Trang cần hiển thị
    /// </summary>
    /// <value></value>
    public int Page { get; set; } = 1;
    /// <summary>
    /// Số dòng hiển thị trên mỗi trang
    /// </summary>
    /// <value></value>
    public int PageSize { get; set; }
    /// <summary>
    /// Chuỗi giá trị cần tìm kiếm
    /// </summary>
    /// <value></value>
    public string SearchValue { get; set; } = "";
  }
}