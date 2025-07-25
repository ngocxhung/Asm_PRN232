using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LibraryManagement_WebApp.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            using var client = new HttpClient();
            var loginData = new { Username, Password };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:5001/api/auth/login", content); // sửa URL nếu khác

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);
                var token = doc.RootElement.GetProperty("token").GetString();
                HttpContext.Session.SetString("JWToken", token);

                var role = doc.RootElement.GetProperty("user").GetProperty("role").GetString();
                HttpContext.Session.SetString("UserRole", role);
                var userId = doc.RootElement.GetProperty("user").GetProperty("userId").GetInt32();
                HttpContext.Session.SetString("UserId", userId.ToString());

                // ✅ Điều hướng theo role
                if (role == "Admin")
                {
                    return RedirectToPage("/Admin");
                }
                else
                {
                    return RedirectToPage("/UserPage/Home"); // hoặc bất kỳ page phù hợp
                }
            }
            else
            {
                ErrorMessage = "Sai tài khoản hoặc mật khẩu!";
                return Page();
            }
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
} 