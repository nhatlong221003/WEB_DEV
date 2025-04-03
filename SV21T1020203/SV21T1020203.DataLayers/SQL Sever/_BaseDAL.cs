using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
namespace SV21T1020203.DataLayers.SQLSever
{
  public class BaseDAL
  {
    protected string _connectionString = "";

    /// <summary>
    /// Cons (hàm tạo)
    /// </summary>
    /// <param name="connectionString"></param>
    public BaseDAL(string connectionString)
    {
      _connectionString = connectionString;
    }
    /// <summary>
    /// tạo và mở kết nối đến CSDL
    /// </summary>
    /// <returns></returns>
    protected SqlConnection OpenConnection()
    {
      SqlConnection connection = new SqlConnection();
      connection.ConnectionString = _connectionString;
      connection.Open();
      return connection;
    }
  }
}

