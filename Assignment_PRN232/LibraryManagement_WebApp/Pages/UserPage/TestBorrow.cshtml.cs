using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using BussinessObjects.Models;

namespace LibraryManagement_WebApp.Pages.UserPage
{
    public class TestBorrowModel : PageModel
    {
        public string? UserId { get; set; }
        public string? UserRole { get; set; }
        public bool HasToken { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public List<BorrowRecord> CurrentBorrows { get; set; } = new List<BorrowRecord>();

        public async Task OnGetAsync()
        {
            // Check session
            UserId = HttpContext.Session.GetString("UserId") ?? "Not set";
            UserRole = HttpContext.Session.GetString("UserRole") ?? "Not set";
            HasToken = !string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken"));

            // Get current borrows
            await LoadCurrentBorrows();
        }

        public async Task<IActionResult> OnPostAsync(int BookId)
        {
            // Check session
            UserId = HttpContext.Session.GetString("UserId") ?? "Not set";
            UserRole = HttpContext.Session.GetString("UserRole") ?? "Not set";
            HasToken = !string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken"));

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                Message = "Vui lòng đăng nhập để mượn sách";
                IsSuccess = false;
                return Page();
            }

            try
            {
                using var client = CreateClient();
                
                // Create borrow record
                var borrowData = new
                {
                    UserId = userId,
                    BookId = BookId,
                    BorrowDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(14),
                    Status = "Borrowed"
                };
                
                var content = new StringContent(JsonSerializer.Serialize(borrowData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:5001/api/borrows/user", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    Message = $"Mượn sách thành công! Response: {responseJson}";
                    IsSuccess = true;
                }
                else
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    Message = $"Mượn sách thất bại! Status: {response.StatusCode}, Error: {errorJson}";
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                Message = $"Exception: {ex.Message}";
                IsSuccess = false;
            }

            // Reload current borrows
            await LoadCurrentBorrows();
            
            return Page();
        }

        private async Task LoadCurrentBorrows()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                try
                {
                    using var client = CreateClient();
                    var response = await client.GetAsync($"https://localhost:5001/api/borrows/user/current/{userId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        CurrentBorrows = JsonSerializer.Deserialize<List<BorrowRecord>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<BorrowRecord>();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading borrows: {ex.Message}");
                }
            }
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