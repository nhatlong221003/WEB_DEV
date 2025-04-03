using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV21T1020203.Shop
{
  public class WebUserData
  {
    public string UserId { get; set; } = "";
    public string UserName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Photo { get; set; } = "";
    public List<string> Roles { get; set; }

    public ClaimsPrincipal CreatePrincipal()
    {
      List<Claim> claims = new List<Claim>()
            {
                new Claim(nameof(UserId), UserId),
                new Claim(nameof(UserName), UserName),
                new Claim(nameof(DisplayName), DisplayName),
                new Claim(nameof(Photo), Photo),
            };
      if (Roles != null)
        foreach (var role in Roles)
          claims.Add(new Claim(ClaimTypes.Role, role));
      // Taoj Identity dựa trên các thông tin có trong danh sách cá Claim
      var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

      //Tạo Princiapal (giấy chứng nhận => thẻ sinh viên)
      var principal = new ClaimsPrincipal(identity);

      return principal;
    }
  }
}
