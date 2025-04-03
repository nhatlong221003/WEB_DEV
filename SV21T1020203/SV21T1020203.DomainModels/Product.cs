using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020203.DomainModels
{
  public class Product
  {
    public int ProductID { get; set; }
    public string ProductName { get; set; } = "";
    public string ProductDescription { get; set; } = "";
    public int SupplierID { get; set; }
    public int CategoryID { get; set; }
    public string Unit { get; set; } = "";
    public decimal Price { get; set; }
    public string Photo { get; set; } = "";
    public bool IsSelling { get; set; }

    public string CategoryName { get; set; } = "";
    public string SupplierName { get; set; } = "";
  }

  public class ProductPhoto
  {
    public long PhotoID { get; set; }
    public int ProductID { get; set; }
    public string Photo { get; set; } = "";
    public string Description { get; set; } = "";
    public int DisplayOrder { get; set; }
    public bool IsHidden { get; set; }
  }

  public class ProductAttribute
  {
    public long AttributeID { get; set; }
    public int ProductID { get; set; }
    public string AttributeName { get; set; } = "";
    public string AttributeValue { get; set; } = "";
    public int DisplayOrder { get; set; }
  }
}
