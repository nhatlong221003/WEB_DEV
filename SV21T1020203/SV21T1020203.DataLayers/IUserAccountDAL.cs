
using SV21T1020203.DomainModels;

namespace SV21T1020203.DataLayers
{
  public interface IUserAccountDAL
  {
    /// <summary>
    /// kiểm tra xem tên đăng nhập và mật khẩu có đúng hay không?
    /// nếu đúng trả về thông tin user ,nếu sai trả về null
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    UserAccount? Authorize(string username, string password);
    /// <summary>
    /// đổi mật khẩu
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    bool ChangePassword(string username, string oldpassword, string newpassword);
    bool Register(Customer data, string username, string password);

  }
}
