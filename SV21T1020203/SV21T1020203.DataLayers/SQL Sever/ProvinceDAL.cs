
using Dapper;
using SV21T1020203.DataLayers;
using SV21T1020203.DataLayers.SQLSever;
using SV21T1020203.DomainModels;

namespace SV21T1020203.DataLayers.SQLServer
{
  public class ProvinceDAL : BaseDAL, ISimpleQueryDAL<Province>
  {
    public ProvinceDAL(string connectionString) : base(connectionString)
    {

    }

    public List<Province> List()
    {
      List<Province> data = new List<Province>();
      using (var connection = OpenConnection())
      {
        var sql = @"select * from Provinces";
        data = connection.Query<Province>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
        connection.Close();
      }
      return data;
    }
  }

}