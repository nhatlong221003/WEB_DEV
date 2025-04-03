


using Dapper;
using SV21T1020203.DataLayers.SQLSever;
using SV21T1020203.DomainModels;
using System.Data;

namespace SV21T1020203.DataLayers.SQLServer
{
  public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
  {
    public CustomerAccountDAL(string connectionString) : base(connectionString)
    {
    }

    public UserAccount? Authorize(string username, string password)
    {
      UserAccount? data = null;
      using (var connection = OpenConnection())
      {
        var sql = @"select CustomerID as UserId,
                                   Email as UserName,
                                   CustomerName as DisplayName,
                                   N'' as Photo,
                                   N'' as RoleNames
                            from   Customers
                            where  Email=@Email and Password=@Password
                    
                            ";
        var parameters = new
        {
          Email = username,
          Password = password

        };
        data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: CommandType.Text);
        connection.Close();
      }
      return data;
    }

    public bool ChangePassword(string username, string oldpassword, string newpassword)
    {
      bool result = false;
      // so sánh mật khẩu cũ trong database
      UserAccount userAccount = Authorize(username, oldpassword);
      if (userAccount == null) return result;
      using (var connection = OpenConnection())
      {
        // cập nhật mật khẩu mới
        var sql = @"UPDATE  Customers
                    SET     Password = @Password
                    WHERE   Email = @Email";

        var parameters = new
        {
          Email = username,
          Password = newpassword
        };
        result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
        connection.Close();
      }
      return result;
    }

    public bool Register(Customer data, string username, string password)
    {
      bool result = false;
      using (var connection = OpenConnection())
      {
        var sql = @"INSERT INTO Customers(CustomerName,ContactName,Address,Phone,Email,Password)
                            VALUES (@CustomerName,@ContactName,@Address,@Phone,@Email,@Password)"
        ;

        var parameters = new
        {
          CustomerName = data.CustomerName ?? "",
          ContactName = data.ContactName ?? "",
          Address = data.Address ?? "",
          Phone = data.Phone ?? "",
          Email = username ?? "",
          Password = password ?? ""
        };

        result = connection.Execute(sql, parameters, commandType: CommandType.Text) > 0;
      }
      return result;
    }
  }
}
