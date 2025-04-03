namespace SV21T1020203.DataLayers
{
  /// <summary>
  /// Định nghĩa chức năng truy vấn dữ liệu đơn giản
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface ISimpleQueryDAL<T> where T : class
  {
    /// <summary>
    /// Truy vấn và lấy toàn bộ dữ liệu của bảng
    /// </summary>
    /// <returns></returns>
    List<T> List();
  }
}