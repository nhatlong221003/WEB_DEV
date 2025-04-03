using Dapper;
using SV21T1020203.DataLayers.SQLSever;
using SV21T1020203.DomainModels;


namespace SV21T1020203.DataLayers.SQLServer
{
  public class ProductDAL : BaseDAL, IProductDAL
  {
    public ProductDAL(string connectionString) : base(connectionString)
    {
    }

    public int Add(Product data)
    {
      int id = 0;
      using (var connection = OpenConnection())
      {
        var sql = @"if exists(select * from Products where ProductName = @ProductName)
                                select -1
                            else if not exists(select * from Categories where CategoryID = @CategoryID)
                                select -2
                            else if not exists(select * from Suppliers where SupplierID = @SupplierID)
                                select -3
                            else
                                begin
                                    insert into Products(ProductName,ProductDescription,SupplierID,CategoryID,Unit,Price,Photo,IsSelling)
                                    values (@ProductName,@ProductDescription,@SupplierID,@CategoryID,@Unit,@Price,@Photo,@IsSelling);
                                    select @@IDENTITY;
                                end";
        var parameters = new
        {
          ProductName = data.ProductName ?? "",
          ProductDescription = data.ProductDescription ?? "",
          SupplierID = data.SupplierID,
          CategoryID = data.CategoryID,
          Unit = data.Unit ?? "",
          Price = data.Price,
          Photo = data.Photo ?? "",
          IsSelling = data.IsSelling


        };
        id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        connection.Close();
      }
      return id;
    }

    public long AddAttribute(ProductAttribute data)
    {
      int id = 0;
      using (var connection = OpenConnection())
      {
        var sql = @"if exists(select * from ProductAttributes where (AttributeID <> @AttributeID and ProductID = @ProductID)
                                                                   and (AttributeName = @AttributeName or DisplayOrder = @DisplayOrder))
                                select -1
                            else
                            begin
                                if exists(select * from Products where ProductID = @ProductID)
                                begin
                                    insert into ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
									values (@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);

                                    select @@identity;
                                end
                                else
                                begin
                                    select -1;
                                end
                            end";
        var parameters = new
        {
          ProductID = data.ProductID,
          AttributeName = data.AttributeName ?? "",
          AttributeValue = data.AttributeValue ?? "",
          DisplayOrder = data.DisplayOrder,
          AttributeID = data.AttributeID,
        };
        id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        connection.Close();
      }
      return id;
    }

    public long AddPhoto(ProductPhoto data)
    {
      int id = 0;

      using (var connection = OpenConnection())
      {
        var sql = @"if exists(select * from ProductPhotos where Photo = @Photo)
	                            select -1
                            else if exists(select * from ProductPhotos where ProductID = @ProductID and (Photo = @Photo or DisplayOrder = @DisplayOrder))
                                select -2
                            else 
	                            begin
                                    insert into ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                                    values (@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                                    select @@IDENTITY; 
                                end";
        var parameters = new
        {
          ProductID = data.ProductID,
          PhotoID = data.PhotoID,
          Photo = data.Photo ?? "",
          Description = data.Description ?? "",
          DisplayOrder = data.DisplayOrder,
          IsHidden = data.IsHidden

        };
        id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        connection.Close();
      }

      return id;
    }

    public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
    {
      int count = 0;
      searchValue = $"%{searchValue}%";
      using (var connection = OpenConnection())
      {
        var sql = @"SELECT COUNT(*) 
                            FROM Products 
                            WHERE (ProductName LIKE @SearchValue OR @SearchValue = N'')
                                AND (CategoryID = @CategoryID OR @CategoryID = 0)
                                AND (SupplierID = @SupplierID OR @SupplierID = 0)
                                AND (Price >= @MinPrice OR @MinPrice = 0)
                                AND (Price <= @MaxPrice OR @MaxPrice = 0)";
        var parameters = new
        {
          searchValue = searchValue,
          CategoryID = categoryID,
          SupplierID = supplierID,
          MinPrice = minPrice,
          MaxPrice = maxPrice
        };
        count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        connection.Close();
      }
      return count;
    }

    public bool Delete(int productID)
    {
      bool result = false;
      using (var connection = OpenConnection())
      {
        var sql = @"
                delete from ProductAttributes where ProductID = @ProductID;
                delete from ProductPhotos where ProductID = @ProductID;
                delete from Products where ProductID = @ProductID;
                ";
        var parameters = new
        {
          ProductID = productID,
        };
        result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
        connection.Close();
      }
      return result;
    }

    public bool DeleteAttribute(long attributeID)
    {
      bool result = false;
      using (var connection = OpenConnection())
      {
        var sql = @"delete from ProductAttributes where AttributeID = @AttributeID;";
        var parameters = new
        {
          AttributeID = attributeID,
        };
        result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
        connection.Close();
      }
      return result;
    }

    public bool DeletePhoto(long photoID)
    {
      bool result = false;
      using (var connection = OpenConnection())
      {
        var sql = @"delete from ProductPhotos where PhotoID = @PhotoID;";
        var parameters = new
        {
          PhotoID = photoID,
        };
        result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
        connection.Close();
      }
      return result;
    }

    public Product? Get(int productID)
    {
      Product? data = null;
      using (var connection = OpenConnection())
      {
        var sql = @"select * from Products where ProductID = @ProductID";
        var parameters = new
        {
          ProductID = productID
        };
        data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        connection.Close();
      }
      return data;
    }

    public ProductAttribute? GetAttribute(long attributeID)
    {
      ProductAttribute? data = null;
      using (var connection = OpenConnection())
      {
        var sql = @"select * from ProductAttributes where AttributeID = @AttributeID";
        var parameters = new
        {
          AttributeID = attributeID
        };
        data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        connection.Close();
      }
      return data;
    }

    public ProductPhoto? GetPhoto(long photoID)
    {
      ProductPhoto? data = null;
      using (var connection = OpenConnection())
      {
        var sql = @"select * from ProductPhotos where PhotoID = @PhotoID";
        var parameters = new
        {
          PhotoID = photoID
        };
        data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        connection.Close();
      }
      return data;
    }

    public bool InUsed(int productID)
    {
      bool result = false;
      using (var connection = OpenConnection())
      {
        var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
	                            select 1
                            else 
	                            select 0";
        var parameters = new
        {
          ProductID = productID
        };
        result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
        connection.Close();
      }
      return result;
    }

    public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
    {
      List<Product> data = new List<Product>();
      searchValue = $"%{searchValue}%"; // Tìm kiếm tương đối với LIKE

      using (var connection = OpenConnection())
      {
        var sql = @"select *
                            from	(
			                            select *,
				                            row_number() over(order by ProductName) as RowNumber
			                            from Products 
			                            where (@searchValue = N'' or ProductName like @searchValue)
											AND (CategoryID = @CategoryID OR @CategoryID = 0)
                                            AND (SupplierID = @SupplierID OR @SupplierID = 0)
											AND (Price >= @MinPrice OR @MinPrice = 0)
											AND (Price <= @MaxPrice OR @MaxPrice = 0)
		                            ) as t
                            where (@pageSize = 0)
	                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
        var parameters = new
        {
          searchValue = searchValue,
          categoryID = categoryID,
          supplierID = supplierID,
          MinPrice = minPrice,
          MaxPrice = maxPrice,
          pageSize = pageSize,
          page = page,
        };
        data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
        connection.Close();
      }

      return data;
    }

    public List<Product> List(string searchValue = "")
    {
      List<Product> data = new List<Product>();

      using (var connection = OpenConnection())
      {
        var sql = @"select * from Products where (@searchValue = N'' or ProductName like @searchValue)";
        var parameters = new
        {
          searchValue = searchValue,
        };
        data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
        connection.Close();
      }

      return data;
    }

    public IList<ProductAttribute> ListAttributes(int productID)
    {
      List<ProductAttribute> data = new List<ProductAttribute>();

      using (var connection = OpenConnection())
      {
        var sql = @"select * from ProductAttributes where ProductID = @ProductID order by DisplayOrder";
        var parameters = new
        {
          ProductID = productID,
        };
        data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
        connection.Close();
      }

      return data;
    }

    public IList<ProductPhoto> ListPhotos(int productID)
    {
      List<ProductPhoto> data = new List<ProductPhoto>();

      using (var connection = OpenConnection())
      {
        var sql = @"select * from ProductPhotos where ProductID = @ProductID order by DisplayOrder";
        var parameters = new
        {
          ProductID = productID,
        };
        data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
        connection.Close();
      }

      return data;
    }

    public bool Update(Product data)
    {
      bool result = false;

      using (var connection = OpenConnection())
      {
        var sql = @"if not exists(select * from Products where ProductID <> @ProductID and ProductName = @ProductName)
                                begin
                                    if exists(select * from Categories where CategoryID = @CategoryID) 
                                    and exists(select * from Suppliers where SupplierID = @SupplierID)
                                    begin
                                        update Products
                                        set ProductName = @ProductName, 
                                            ProductDescription = @ProductDescription, 
                                            SupplierID = @SupplierID, 
                                            CategoryID = @CategoryID, 
                                            Unit = @Unit, 
                                            Price = @Price, 
                                            Photo = @Photo,
                                            IsSelling = @IsSelling
                                        where ProductID = @ProductID;
                                    end
                                    else
                                    begin
                                        select -2; 
                                    end
                                end
                                else
                                begin
                                    select -1; 
                                end";
        var parameters = new
        {
          ProductName = data.ProductName ?? "",
          ProductDescription = data.ProductDescription ?? "",
          SupplierID = data.SupplierID,
          CategoryID = data.CategoryID,
          Unit = data.Unit ?? "",
          Price = data.Price,
          Photo = data.Photo ?? "",
          IsSelling = data.IsSelling,
          ProductID = data.ProductID,
        };
        result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
        connection.Close();
      }

      return result;
    }

    public bool UpdateAttribute(ProductAttribute data)
    {
      bool result = false;

      using (var connection = OpenConnection())
      {
        var sql = @"if exists(select * from ProductAttributes where (AttributeID <> @AttributeID and ProductID = @ProductID)
                                                                   and (AttributeName = @AttributeName or DisplayOrder = @DisplayOrder))
                                select -1
                            else
                            begin
                                if exists(select * from Products where ProductID = @ProductID)
                                begin
                                    update ProductAttributes
                                    set ProductID = @ProductID, 
                                        AttributeName = @AttributeName,
                                        AttributeValue = @AttributeValue, 
                                        DisplayOrder = @DisplayOrder
                                    where AttributeID = @AttributeID;
                                end
                            end";
        var parameters = new
        {
          ProductID = data.ProductID,
          AttributeName = data.AttributeName ?? "",
          AttributeValue = data.AttributeValue ?? "",
          DisplayOrder = data.DisplayOrder,
          AttributeID = data.AttributeID,
        };
        result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
        connection.Close();
      }

      return result;
    }

    public bool UpdatePhoto(ProductPhoto data)
    {
      bool result = false;

      using (var connection = OpenConnection())
      {
        var sql = @"if exists(select * from ProductPhotos where (PhotoID <> @PhotoID and ProductID = @ProductID)
                                                               and (Photo = @Photo or DisplayOrder = @DisplayOrder))
                                select -1
                            
                            else
                            begin
                                if exists(select * from Products where ProductID = @ProductID)
                                begin
                                    update ProductPhotos
                                    set ProductID = @ProductID, 
										Photo = @Photo,
	                                    Description = @Description, 
	                                    DisplayOrder = @DisplayOrder,
										IsHidden = @IsHidden
                                    where PhotoID = @PhotoID
                                end
                                else
                                begin
                                    select -1;
                                end
                            end";
        var parameters = new
        {
          PhotoID = data.PhotoID,
          ProductID = data.ProductID,
          Photo = data.Photo ?? "",
          Description = data.Description ?? "",
          DisplayOrder = data.DisplayOrder,
          IsHidden = data.IsHidden,

        };
        result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
        connection.Close();
      }

      return result;
    }
  }
}
