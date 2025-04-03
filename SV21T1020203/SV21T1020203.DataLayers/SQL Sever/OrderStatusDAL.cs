using Dapper;
using SV21T1020203.DataLayers.SQLSever;
using SV21T1020203.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020203.DataLayers.SQLServer
{
  public class OrderStatusDAL : BaseDAL, ISimpleSelectDAL<OrderStatus>
  {
    public OrderStatusDAL(string connectionString) : base(connectionString)
    {
    }

    public List<OrderStatus> List()
    {
      List<OrderStatus> data = new List<OrderStatus>();
      using (var connection = OpenConnection())
      {
        var sql = "select * from OrderStatus";

        data = connection.Query<OrderStatus>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
        connection.Close();
      }
      return data;
    }
  }

  internal interface ISimpleSelectDAL<T>
  {
  }
}
