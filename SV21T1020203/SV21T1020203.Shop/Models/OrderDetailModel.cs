using SV21T1020203.DomainModels;

namespace SV21T1020203.Shop.Models
{
  public class OrderDetailModel
  {
    public Order? Order { get; set; }
    public required List<OrderDetail> Details { get; set; }
  }
}
