namespace LibraryManagement_WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // ✅ Thêm cấu hình session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian session tồn tại
                options.Cookie.HttpOnly = true; // bảo mật cookie
                options.Cookie.IsEssential = true; // bắt buộc để session hoạt động
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // ✅ Bắt buộc phải có trước UseAuthorization
            app.UseSession(); // << Bật session ở middleware

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
