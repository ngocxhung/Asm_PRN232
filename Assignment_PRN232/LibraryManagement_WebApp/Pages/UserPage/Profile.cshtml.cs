using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using BussinessObjects.Models;
using System.Threading.Tasks;

namespace LibraryManagement_WebApp.Pages.UserPage
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public BussinessObjects.Models.User UserModel { get; set; }
        public string ErrorMessage { get; set; }
        private string _oldPasswordHash;
        private string _oldRole;
        private DateTime _oldCreatedAt;
        private bool _oldStatus;

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return RedirectToPage("/Login");
            using var client = CreateClient();
            var res = await client.GetAsync($"https://localhost:5001/api/users/{userId}");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                UserModel = JsonSerializer.Deserialize<BussinessObjects.Models.User>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                _oldPasswordHash = UserModel.PasswordHash;
                _oldRole = string.IsNullOrEmpty(UserModel.Role) ? "Reader" : UserModel.Role;
                _oldCreatedAt = UserModel.CreatedAt;
                _oldStatus = UserModel.Status;
                if (string.IsNullOrEmpty(UserModel.Role))
                    UserModel.Role = _oldRole;
            }
            else
            {
                ErrorMessage = "Không lấy được thông tin người dùng!";
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return RedirectToPage("/Login");
            // Giữ nguyên password, role, createdAt, status nếu không nhập
            if (string.IsNullOrEmpty(UserModel.PasswordHash))
                UserModel.PasswordHash = _oldPasswordHash;
            if (string.IsNullOrEmpty(UserModel.Role))
                UserModel.Role = _oldRole ?? "Reader";
            UserModel.CreatedAt = _oldCreatedAt;
            UserModel.Status = _oldStatus;
            using var client = CreateClient();
            var json = JsonSerializer.Serialize(UserModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await client.PutAsync($"https://localhost:5001/api/users/{userId}", content);
            if (!res.IsSuccessStatusCode)
            {
                var errorDetail = await res.Content.ReadAsStringAsync();
                ErrorMessage = $"Lưu thất bại! {errorDetail}";
                return Page();
            }
            return RedirectToPage();
        }

        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
} 