

namespace SV21T1020203.Web.Models
{
  public class ProductSearchInput : PaginationSearchInput
  {
    public int CategoryID { get; set; } = 0;
    public int SupplierID { get; set; } = 0;

    public decimal MinPrice { get; set; } = 0;
    public decimal MaxPrice { get; set; } = 0;

  }
}