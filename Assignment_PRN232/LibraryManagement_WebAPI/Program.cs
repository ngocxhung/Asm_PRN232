using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BussinessObjects.Models;

namespace LibraryManagement_WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Đăng ký DbContext
            builder.Services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

            // Cấu hình JWT

            // Đăng ký IUserRepository và UserRepository vào DI container để inject vào controller.
            builder.Services.AddScoped<DataAcess.UserDAO>();
            builder.Services.AddScoped<Repositories.IUserRepository, Repositories.UserRepository>();
            builder.Services.AddScoped<LibraryManagement_WebAPI.Services.IJwtService, LibraryManagement_WebAPI.Services.JwtService>();
            builder.Services.AddScoped<DataAcess.AuthorDAO>();
            builder.Services.AddScoped<Repositories.IAuthorRepository, Repositories.AuthorRepository>();
            builder.Services.AddScoped<DataAcess.PublisherDAO>();
            builder.Services.AddScoped<Repositories.IPublisherRepository, Repositories.PublisherRepository>();
            builder.Services.AddScoped<DataAcess.CategoryDAO>();
            builder.Services.AddScoped<Repositories.ICategoryRepository, Repositories.CategoryRepository>();
            builder.Services.AddScoped<DataAcess.BookDAO>();
            builder.Services.AddScoped<Repositories.IBookRepository, Repositories.BookRepository>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add authentication and authorization
            builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Seed database with admin user
            SeedDatabase(app);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static void SeedDatabase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BussinessObjects.Models.LibraryDbContext>();
            
            // Ensure database is created
            context.Database.EnsureCreated();
            
            // Check if admin user exists
            if (!context.Users.Any(u => u.Username == "admin"))
            {
                var adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // Use a strong password
                    FullName = "System Administrator",
                    Email = "admin@library.com",
                    Phone = "0000000000",
                    Role = "Admin",
                    Status = true,
                    CreatedAt = DateTime.Now,
                    ImageUrl = "default-user.png"
                };

                context.Users.Add(adminUser);
            }

            // Add sample reader user
            if (!context.Users.Any(u => u.Username == "reader"))
            {
                var readerUser = new User
                {
                    Username = "reader",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("reader123"),
                    FullName = "Nguyễn Văn Đọc",
                    Email = "reader@library.com",
                    Phone = "0123456789",
                    Role = "Reader",
                    Status = true,
                    CreatedAt = DateTime.Now,
                    ImageUrl = "default-user.png"
                };

                context.Users.Add(readerUser);
            }

            // Add sample categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { CategoryName = "Tiểu thuyết", Description = "Sách tiểu thuyết" },
                    new Category { CategoryName = "Khoa học", Description = "Sách khoa học" },
                    new Category { CategoryName = "Lịch sử", Description = "Sách lịch sử" },
                    new Category { CategoryName = "Công nghệ", Description = "Sách công nghệ" },
                    new Category { CategoryName = "Văn học", Description = "Sách văn học" }
                };
                context.Categories.AddRange(categories);
            }

            // Add sample authors
            if (!context.Authors.Any())
            {
                var authors = new List<Author>
                {
                    new Author { AuthorName = "Nguyễn Du", Description = "Đại thi hào dân tộc" },
                    new Author { AuthorName = "Nam Cao", Description = "Nhà văn hiện thực" },
                    new Author { AuthorName = "Tô Hoài", Description = "Nhà văn thiếu nhi" },
                    new Author { AuthorName = "Nguyễn Nhật Ánh", Description = "Nhà văn đương đại" },
                    new Author { AuthorName = "Stephen King", Description = "Nhà văn kinh dị Mỹ" }
                };
                context.Authors.AddRange(authors);
            }

            // Add sample publishers
            if (!context.Publishers.Any())
            {
                var publishers = new List<Publisher>
                {
                    new Publisher { PublisherName = "NXB Văn học",  ContactInfo = "0123456789" },
                    new Publisher { PublisherName = "NXB Kim Đồng", ContactInfo = "0123456790" },
                    new Publisher { PublisherName = "NXB Trẻ", ContactInfo = "0123456791" },
                    new Publisher { PublisherName = "NXB Giáo dục", ContactInfo = "0123456792" }
                };
                context.Publishers.AddRange(publishers);
            }

            context.SaveChanges();

            // Add sample books
            if (!context.Books.Any())
            {
                var categories = context.Categories.ToList();
                var authors = context.Authors.ToList();
                var publishers = context.Publishers.ToList();

                var books = new List<Book>
                {
                    new Book
                    {
                        Title = "Truyện Kiều",
                        AuthorId = authors[0].AuthorId,
                        CategoryId = categories[4].CategoryId, // Văn học
                        PublisherId = publishers[0].PublisherId,
                        PublishYear = 1820,
                        ISBN = "978-604-321-001-1",
                        Description = "Tác phẩm văn học kinh điển của Việt Nam",
                        Quantity = 10,
                        Available = 8,
                        Location = "Kệ A1",
                        Status = "Available",
                        ImageUrl = "truyen-kieu.jpg"
                    },
                    new Book
                    {
                        Title = "Chí Phèo",
                        AuthorId = authors[1].AuthorId,
                        CategoryId = categories[4].CategoryId, // Văn học
                        PublisherId = publishers[0].PublisherId,
                        PublishYear = 1941,
                        ISBN = "978-604-321-002-2",
                        Description = "Tác phẩm văn học hiện thực phê phán",
                        Quantity = 5,
                        Available = 3,
                        Location = "Kệ A2",
                        Status = "Available",
                        ImageUrl = "chi-pheo.jpg"
                    },
                    new Book
                    {
                        Title = "Dế Mèn Phiêu Lưu Ký",
                        AuthorId = authors[2].AuthorId,
                        CategoryId = categories[0].CategoryId, // Tiểu thuyết
                        PublisherId = publishers[1].PublisherId,
                        PublishYear = 1941,
                        ISBN = "978-604-321-003-3",
                        Description = "Tác phẩm văn học thiếu nhi nổi tiếng",
                        Quantity = 8,
                        Available = 6,
                        Location = "Kệ B1",
                        Status = "Available",
                        ImageUrl = "de-men.jpg"
                    },
                    new Book
                    {
                        Title = "Tôi Thấy Hoa Vàng Trên Cỏ Xanh",
                        AuthorId = authors[3].AuthorId,
                        CategoryId = categories[0].CategoryId, // Tiểu thuyết
                        PublisherId = publishers[2].PublisherId,
                        PublishYear = 2010,
                        ISBN = "978-604-321-004-4",
                        Description = "Tiểu thuyết tuổi thơ đầy cảm xúc",
                        Quantity = 12,
                        Available = 10,
                        Location = "Kệ B2",
                        Status = "Available",
                        ImageUrl = "toi-thay-hoa-vang.jpg"
                    },
                    new Book
                    {
                        Title = "The Shining",
                        AuthorId = authors[4].AuthorId,
                        CategoryId = categories[0].CategoryId, // Tiểu thuyết
                        PublisherId = publishers[0].PublisherId,
                        PublishYear = 1977,
                        ISBN = "978-604-321-005-5",
                        Description = "Tiểu thuyết kinh dị nổi tiếng",
                        Quantity = 6,
                        Available = 4,
                        Location = "Kệ C1",
                        Status = "Available",
                        ImageUrl = "the-shining.jpg"
                    }
                };
                context.Books.AddRange(books);
            }

            context.SaveChanges();
        }
    }
}
