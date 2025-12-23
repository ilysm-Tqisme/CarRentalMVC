using CarRentalMVC.Services;

namespace CarRentalMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // ✅ Cấu hình session để lưu JWT token
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // ✅ Thêm HttpContextAccessor để truy cập Session
            builder.Services.AddHttpContextAccessor();

            // ✅ Thêm ApiService để gọi API backend
            builder.Services.AddScoped<ApiService>();

            // ✅ Thêm HttpClient
            builder.Services.AddHttpClient();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseSession(); // ✅ phải có dòng này
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");




            app.Run();
        }
    }
}
