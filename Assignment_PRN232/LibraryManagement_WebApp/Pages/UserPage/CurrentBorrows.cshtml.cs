using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using BussinessObjects.Models;

namespace LibraryManagement_WebApp.Pages.UserPage
{
    public class CurrentBorrowsModel : PageModel
    {
        public List<BorrowRecord> CurrentBorrows { get; set; } = new List<BorrowRecord>();
        public string? Message { get; set; }
        public string? MessageType { get; set; }

        public async Task OnGetAsync()
        {
            // Get message from TempData if any
            if (TempData.ContainsKey("Message"))
            {
                Message = TempData["Message"]?.ToString();
                MessageType = TempData["MessageType"]?.ToString();
            }
            
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                Message = "Vui lòng đăng nhập để xem sách đang mượn";
                MessageType = "warning";
                return;
            }

            using var client = CreateClient();
            var response = await client.GetAsync($"https://localhost:5001/api/borrows/user/current/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                CurrentBorrows = JsonSerializer.Deserialize<List<BorrowRecord>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<BorrowRecord>();
            }
        }

        public async Task<IActionResult> OnPostExtendAsync(int borrowId)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                Message = "Vui lòng đăng nhập để thực hiện thao tác";
                MessageType = "danger";
                return RedirectToPage();
            }

            using var client = CreateClient();
            
            // Debug info
            System.Diagnostics.Debug.WriteLine($"Extend borrow: borrowId={borrowId}, userId={userId}");
            
            var response = await client.PutAsync($"https://localhost:5001/api/borrows/{borrowId}/extend", null);
            System.Diagnostics.Debug.WriteLine($"Extend Response: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Extend Success: {result}");
                TempData["Message"] = "Gia hạn thành công!";
                TempData["MessageType"] = "success";
            }
            else
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Extend API Error: {response.StatusCode} - {errorJson}");
                TempData["Message"] = $"Gia hạn thất bại: {errorJson}";
                TempData["MessageType"] = "danger";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostReturnAsync(int borrowId)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                Message = "Vui lòng đăng nhập để thực hiện thao tác";
                MessageType = "danger";
                return RedirectToPage();
            }

            using var client = CreateClient();
            
            // Debug info  
            System.Diagnostics.Debug.WriteLine($"Return borrow: borrowId={borrowId}, userId={userId}");
            
            var response = await client.PutAsync($"https://localhost:5001/api/borrows/{borrowId}/return", null);
            System.Diagnostics.Debug.WriteLine($"Return Response: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Return Success: {result}");
                TempData["Message"] = "Trả sách thành công!";
                TempData["MessageType"] = "success";
            }
            else
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Return API Error: {response.StatusCode} - {errorJson}");
                TempData["Message"] = $"Trả sách thất bại: {errorJson}";
                TempData["MessageType"] = "danger";
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