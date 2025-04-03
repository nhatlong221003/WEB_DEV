
using SV21T1020203.DataLayers;
using SV21T1020203.DomainModels;

namespace SV21T1020203.BusinessLayers
{
  public class UserAccountService
  {
    private static readonly IUserAccountDAL employeeAccountDB;
    private static readonly IUserAccountDAL customerAccountDB;

    static UserAccountService()

    {
      string connectionString = Configuration.ConnectionString;
      employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
      customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);

    }
    public static UserAccount? Authorize(UserTypes userType, string username, string password)
    {
      if (userType == UserTypes.Employee)
        return employeeAccountDB.Authorize(username, password);
      else
        return customerAccountDB.Authorize(username, password);
    }
    public static bool ChangePassword(UserTypes userType, string username, string oldpassword, string newpassword)
    {
      if (userType == UserTypes.Employee)
        return employeeAccountDB.ChangePassword(username, oldpassword, newpassword);
      else
        return customerAccountDB.ChangePassword(username, oldpassword, newpassword);
    }


    public static bool Register(Customer data, string username, string password)
    {
      return customerAccountDB.Register(data, username, password);
    }


  }
  public enum UserTypes
  {
    Employee,
    Customer
  }
}
