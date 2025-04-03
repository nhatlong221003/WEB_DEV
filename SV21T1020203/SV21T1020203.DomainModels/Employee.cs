namespace SV21T1020203.DomainModels
{
  public class Employee
  {
    public int EmployeeID { get; set; }
    public string FullName { get; set; } = string.Empty;  // Sửa thành string
    public DateTime BirthDate { get; set; }
    public string Address { get; set; } = String.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public bool IsWorking { get; set; }
  }
}
