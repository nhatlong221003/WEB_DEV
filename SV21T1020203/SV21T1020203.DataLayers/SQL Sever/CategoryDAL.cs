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
  public class CategoryDAL : BaseDAL, ICommonDAL<Category>
  {
    public CategoryDAL(string connectionString) : base(connectionString)
    {
    }

    public int Add(Category data)
    {
      int id = 0;
      using (var connection = OpenConnection())
      {
        var sql = @"if exists(select * from Categories where CategoryName = @CategoryName)
                                 select -1
                            else
                                begin
                                    insert into Categories(CategoryName,Description)
                                    values (@CategoryName, @Description)

                                    select SCOPE_IDENTITY();
                             end";
        var parameter = new
        {
          CategoryName = data.CategoryName ?? "",
          Description = data.Description ?? ""
        };
        id = connection.ExecuteScalar<int>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
        connection.Close();
      }
      return id;
    }

    public int Count(string searchValue = "")
    {
      int count = 0;
      searchValue = $"%{searchValue}%";   //"%" + searchValue + "%"
      using (var connection = OpenConnection())
      {
        var sql = @"select count(*)
                            from Categories
                            where (CategoryName like @searchValue)";
        var parameters = new
        {
          searchValue
        };
        count = connection.ExecuteScalar<int>(sql, param: parameters, commandType: System.Data.CommandType.Text);
      }
      return count;
    }

    public bool Delete(int id)
    {
      bool result = false;
      using (var connection = OpenConnection())
      {
        var sql = @"delete from Categories where CategoryID = @CategoryID";
        var parameter = new
        {
          CategoryID = id
        };
        result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
        connection.Close();
      }
      return result;
    }

    public Category? Get(int id)
    {
      Category? data = null;
      using (var connectiom = OpenConnection())
      {
        var sql = @"select * from Categories where CategoryID = @CategoryID";
        var parameter = new
        {
          CategoryID = id
        };
        data = connectiom.QueryFirstOrDefault<Category>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
      }
      return data;
    }

    public bool InUsed(int id)
    {
      bool result = false;
      using (var connection = OpenConnection())
      {
        var sql = @"if exists(select * from Products where CategoryID = @CategoryID)
	                            select 1
                            else
	                            select 0";
        var paramater = new
        {
          CategoryID = id
        };
        result = connection.ExecuteScalar<bool>(sql: sql, param: paramater, commandType: System.Data.CommandType.Text);
        connection.Close();
      }
      return result;
    }

    public List<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
    {
      List<Category> data = new List<Category>();
      searchValue = $"%{searchValue}%";    //tìm kiếm tương đối với LIKE

      using (var connection = OpenConnection())
      {
        var sql = @"select *
                            from (	select *,
				                       row_number() over(order by CategoryName) as RowNumber
		                            from Categories
		                            where (CategoryName like @searchValue)
	                             ) as t
                            where (@pageSize = 0)
	                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
        var parameters = new
        {
          page = page,        //ben trai: ten tham so trong cau lenh SQL, ben phai: gia tri truyen cho tham so
          pageSize = pageSize,
          searchValue = searchValue
        };
        data = connection.Query<Category>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
        connection.Close();
      }
      return data;
    }

    public bool Update(Category data)
    {
      bool result = false;
      using (var connection = OpenConnection())
      {
        var sql = @"if not exists(select * from Categories where CategoryId <> @CategoryId and CategoryName = @CategoryName)
                            begin
                                update Categories
                                set CategoryName = @CategoryName,
	                                Description = @Description
                                where CategoryID = @CategoryID
                            end";
        var parameter = new
        {
          CategoryName = data.CategoryName,
          Description = data.Description,
          CategoryID = data.CategoryID
        };
        result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
        connection.Close();
      }
      return result;
    }
  }
}
