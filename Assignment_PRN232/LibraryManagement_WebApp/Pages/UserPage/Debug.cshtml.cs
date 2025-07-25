using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using BussinessObjects.Models;

namespace LibraryManagement_WebApp.Pages.UserPage
{
    public class DebugModel : PageModel
    {
        public string? SessionUserId { get; set; }
        public string? SessionToken { get; set; }
        public string? SessionRole { get; set; }
        public string? Message { get; set; }
        public string? MessageType { get; set; }
        
        [BindProperty]
        public int TestBookId { get; set; } = 1;
        
        public List<BorrowRecord> CurrentBorrows { get; set; } = new List<BorrowRecord>();

        public async Task OnGetAsync()
        {
            LoadSessionData();
            await LoadCurrentBorrows();
        }

        public async Task<IActionResult> OnPostTestBorrowAsync()
        {
            LoadSessionData();
            
            if (string.IsNullOrEmpty(SessionUserId) || !int.TryParse(SessionUserId, out int userId))
            {
                Message = "Không có session userId";
                MessageType = "danger";
                await LoadCurrentBorrows();
                return Page();
            }

            using var client = CreateClient();
            
            var borrowData = new
            {
                UserId = userId,
                BookId = TestBookId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14)
            };
            
            var content = new StringContent(JsonSerializer.Serialize(borrowData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:5001/api/borrows/user", content);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Message = $"Borrow success: {result}";
                MessageType = "success";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Message = $"Borrow failed ({response.StatusCode}): {error}";
                MessageType = "danger";
            }
            
            await LoadCurrentBorrows();
            return Page();
        }

        public async Task<IActionResult> OnPostTestExtendAsync()
        {
            LoadSessionData();
            await LoadCurrentBorrows();
            
            if (!CurrentBorrows.Any())
            {
                Message = "Không có sách nào để gia hạn";
                MessageType = "warning";
                return Page();
            }
            
            var borrowId = CurrentBorrows.First().BorrowId;
            using var client = CreateClient();
            var response = await client.PutAsync($"https://localhost:5001/api/borrows/{borrowId}/extend", null);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Message = $"Extend success: {result}";
                MessageType = "success";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Message = $"Extend failed ({response.StatusCode}): {error}";
                MessageType = "danger";
            }
            
            await LoadCurrentBorrows();
            return Page();
        }

        public async Task<IActionResult> OnPostTestReturnAsync()
        {
            LoadSessionData();
            await LoadCurrentBorrows();
            
            if (!CurrentBorrows.Any())
            {
                Message = "Không có sách nào để trả";
                MessageType = "warning";
                return Page();
            }
            
            var borrowId = CurrentBorrows.First().BorrowId;
            using var client = CreateClient();
            var response = await client.PutAsync($"https://localhost:5001/api/borrows/{borrowId}/return", null);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Message = $"Return success: {result}";
                MessageType = "success";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Message = $"Return failed ({response.StatusCode}): {error}";
                MessageType = "danger";
            }
            
            await LoadCurrentBorrows();
            return Page();
        }

        private void LoadSessionData()
        {
            SessionUserId = HttpContext.Session.GetString("UserId");
            SessionToken = HttpContext.Session.GetString("JWToken");
            SessionRole = HttpContext.Session.GetString("UserRole");
        }

        private async Task LoadCurrentBorrows()
        {
            if (string.IsNullOrEmpty(SessionUserId) || !int.TryParse(SessionUserId, out int userId))
                return;

            using var client = CreateClient();
            var response = await client.GetAsync($"https://localhost:5001/api/borrows/user/current/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                CurrentBorrows = JsonSerializer.Deserialize<List<BorrowRecord>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<BorrowRecord>();
            }
        }

        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(SessionToken))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SessionToken);
            }
            return client;
        }
    }
} 