using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using BussinessObjects.Models;

namespace LibraryManagement_WebApp.Pages.UserPage
{
    public class FinesModel : PageModel
    {
        public List<Fine> Fines { get; set; } = new List<Fine>();
        public string? Message { get; set; }
        public string? MessageType { get; set; }

        public async Task OnGetAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                Message = "Vui long dang nhap de xem phat";
                MessageType = "warning";
                return;
            }

            using var client = CreateClient();
            var response = await client.GetAsync($"https://localhost:5001/api/fines/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Fines = JsonSerializer.Deserialize<List<Fine>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }

        public async Task<IActionResult> OnPostPayAsync(int fineId)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                Message = "Vui long dang nhap de thuc hien thao tac";
                MessageType = "danger";
                return RedirectToPage();
            }

            using var client = CreateClient();
            var response = await client.PostAsync($"https://localhost:5001/api/fines/{fineId}/pay", null);
            
            if (response.IsSuccessStatusCode)
            {
                Message = "Thanh toan thanh cong!";
                MessageType = "success";
            }
            else
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                Message = $"Thanh toan that bai: {errorJson}";
                MessageType = "danger";
            }

            return RedirectToPage();
        }

        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }
    }
} 