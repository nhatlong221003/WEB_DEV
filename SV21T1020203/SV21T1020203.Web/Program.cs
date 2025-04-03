using Microsoft.AspNetCore.Authentication.Cookies;
using SV21T1020203.Web;


namespace SV21T1020203.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews()
                .AddMvcOptions(option =>
                {
                    option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(option =>
                            {
                                option.Cookie.Name = "AuthenticationCookie"; // tên của cookie
                                option.LoginPath = "/Account/Login"; //URL của trang cookie 
                                option.AccessDeniedPath = "/Account/AccessDenined"; // URL của trang khi người sử dụng không được cấp quyền
                                option.ExpireTimeSpan = TimeSpan.FromDays(360); // Khoảng time tồn tại của cookie
                            });
            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(60);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            ApplicationContext.Configure
                (
                    context: app.Services.GetRequiredService<IHttpContextAccessor>(),
                    enviroment: app.Services.GetRequiredService<IWebHostEnvironment>()

                );
            //khởi tạo cấu hình cho BusinessLayer
            string connectionString = builder.Configuration.GetConnectionString("SV21T1020203_Update2023") ?? throw new Exception("Missing connection string 'SV21T1020203_Update2023' in appsettings.json");
            SV21T1020203.BusinessLayers.Configuration.Initialize(connectionString);

            app.Run();
        }
    }
}