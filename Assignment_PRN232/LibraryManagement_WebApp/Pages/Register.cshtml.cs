using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System;

namespace LibraryManagement_WebApp.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Phone { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = new {
                Username,
                PasswordHash = Password,
                FullName,
                Email,
                Phone,
                Role = "Reader",
                Status = true,
                CreatedAt = DateTime.Now,
                ImageUrl = "default-user.png"
            };
            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:5001/api/users", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Login");
            }
            else
            {
                var apiError = await response.Content.ReadAsStringAsync();
                try
                {
                    var doc = JsonDocument.Parse(apiError);
                    if (doc.RootElement.TryGetProperty("error", out var errorProp))
                        ErrorMessage = errorProp.GetString();
                    else if (doc.RootElement.TryGetProperty("message", out var msgProp))
                        ErrorMessage = msgProp.GetString();
                    else
                        ErrorMessage = apiError;
                }
                catch
                {
                    ErrorMessage = !string.IsNullOrWhiteSpace(apiError) ? apiError : "Đăng ký thất bại!";
                }
                return Page();
            }
        }
    }
} 