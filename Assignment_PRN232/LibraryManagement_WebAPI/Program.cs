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
    }
}
