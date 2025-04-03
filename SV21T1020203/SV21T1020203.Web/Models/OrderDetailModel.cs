using SV21T1020203.DomainModels;

namespace SV21T1020203.Web.Models
{
  public class OrderDetailModel
  {
    public Order? Order { get; set; }
    public required List<OrderDetail> Details { get; set; }
  }
}
