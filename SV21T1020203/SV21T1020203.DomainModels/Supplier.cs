namespace SV21T1020203.DomainModels
{
  public class Supplier
  {
    public int SupplierID { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
  }
}
