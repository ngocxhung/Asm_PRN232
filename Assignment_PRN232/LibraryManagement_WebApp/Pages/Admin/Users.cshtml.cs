using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using BussinessObjects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement_WebApp.Pages.Admin
{
    public class UsersModel : PageModel
    {
        public List<BussinessObjects.Models.User> Users { get; set; }
        [BindProperty]
        public BussinessObjects.Models.User UserModel { get; set; }
        public bool ShowForm { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return RedirectToPage("/Login");
            await LoadUsers();
            return Page();
        }

        public async Task<IActionResult> OnGetAddAsync()
        {
            ShowForm = true;
            UserModel = new BussinessObjects.Models.User { Status = true };
            await LoadUsers();
            return Page();
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            ShowForm = true;
            await LoadUsers();
            using var client = CreateClient();
            var res = await client.GetAsync($"https://localhost:5001/api/users/{id}");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                UserModel = JsonSerializer.Deserialize<BussinessObjects.Models.User>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                ErrorMessage = "Không tìm thấy người dùng!";
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            var isEdit = UserModel.UserId > 0;
            using var client = CreateClient();
            var json = JsonSerializer.Serialize(UserModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage res;
            if (isEdit)
                res = await client.PutAsync($"https://localhost:5001/api/users/{UserModel.UserId}", content);
            else
                res = await client.PostAsync("https://localhost:5001/api/users", content);
            if (!res.IsSuccessStatusCode)
            {
                var errorDetail = await res.Content.ReadAsStringAsync();
                ErrorMessage = $"Lưu thất bại! {errorDetail}";
                ShowForm = true;
                await LoadUsers();
                return Page();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDeleteAsync(int id)
        {
            using var client = CreateClient();
            var res = await client.DeleteAsync($"https://localhost:5001/api/users/{id}");
            if (!res.IsSuccessStatusCode)
                ErrorMessage = "Xóa thất bại!";
            await LoadUsers();
            return RedirectToPage();
        }

        private async Task LoadUsers()
        {
            using var client = CreateClient();
            var res = await client.GetAsync("https://localhost:5001/api/users");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                Users = JsonSerializer.Deserialize<List<BussinessObjects.Models.User>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                Users = new List<BussinessObjects.Models.User>();
                ErrorMessage = "Không lấy được danh sách người dùng!";
            }
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