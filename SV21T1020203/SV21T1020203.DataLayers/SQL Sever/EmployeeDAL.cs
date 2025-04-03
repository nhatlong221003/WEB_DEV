using Dapper;
using SV21T1020203.DataLayers.SQLSever;
using SV21T1020203.DomainModels;

namespace SV21T1020203.DataLayers.SQLServer
{
  public class EmployeeDAL : BaseDAL, ICommonDAL<Employee>
  {
    public EmployeeDAL(string connectionString) : base(connectionString)
    {
    }

    public int Add(Employee data)
    {
      int id = 0;
      using (var connection = OpenConnection())
      {
        var sql = @"if exists( select * from Employees where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Employees(FullName, BirthDate,Address,Phone,Email,Photo,IsWorking)
                                    values (@FullName, @BirthDate, @Address, @Phone, @Email, @Photo, @Isworking);

                                    select @@identity;
                            end";
        var parameter = new
        {
          FullName = data.FullName ?? "",
          BirthDate = data.BirthDate,
          Address = data.Address ?? "",
          Phone = data.Phone ?? "",
          Email = data.Email ?? "",
          Photo = data.Photo ?? "",
          Isworking = data.IsWorking
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
                            from Employees
                            where (FullName like @searchValue)";
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
        var sql = @"delete from Employees where EmployeeID = @EmployeeID";
        var parameter = new
        {
          EmployeeID = id
        };
        result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
        connection.Close();
      }
      return result;
    }

    public Employee? Get(int id)
    {
      Employee? data = null;
      using (var connection = OpenConnection())
      {
        var sql = @"select * from Employees where EmployeeID = @EmployeeID";
        var parameter = new
        {
          EmployeeID = id
        };
        data = connection.QueryFirstOrDefault<Employee>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
        connection.Close();
      }
      return data;
    }

    public bool InUsed(int id)
    {
      bool result = false;
      using (var connection = OpenConnection())
      {
        var sql = @"if exists(select * from Orders where EmployeeID = @EmployeeID)
	                            select 1
                            else
	                            select 0";
        var parameter = new
        {
          EmployeeID = id
        };
        result = connection.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
        connection.Close();
      }
      return result;
    }

    public List<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
    {
      List<Employee> data = new List<Employee>();
      searchValue = $"%{searchValue}%";    //tìm kiếm tương đối với LIKE

      using (var connection = OpenConnection())
      {
        var sql = @"select *
                            from (	select *,
				                       row_number() over(order by FullName) as RowNumber
		                            from Employees
		                            where (FullName like @searchValue)
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
        data = connection.Query<Employee>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
        connection.Close();
      }

      return data;
    }

    public bool Update(Employee data)
    {
      bool result = false;
      using (var connection = OpenConnection())
      {
        var sql = @"if not exists(select * from Employees where EmployeeID <> @EmployeeID and Email = @Email)
                            begin
                                update Employees
                                set FullName = @FullName,
	                                BirthDate = @BirthDate,
	                                Address = @Address,
	                                Phone = @Phone,
	                                Email = @Email,
								    Photo = @Photo,
	                                Isworking = @Isworking
                                where EmployeeID = @EmployeeID
                            end";
        var parameter = new
        {
          FullName = data.FullName,
          BirthDate = data.BirthDate,
          Address = data.Address,
          Phone = data.Phone,
          Email = data.Email,
          Photo = data.Photo,
          Isworking = data.IsWorking,
          EmployeeID = data.EmployeeID
        };
        result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
        connection.Close();
      }
      return result;
    }
  }
}
