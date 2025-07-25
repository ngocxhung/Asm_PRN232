using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using BussinessObjects.Models;

namespace LibraryManagement_WebApp.Pages.UserPage
{
    public class BookDetailModel : PageModel
    {
        [BindProperty]
        public int Id { get; set; }
        
        public Book Book { get; set; } = null!;
        public bool CanBorrow { get; set; } = true;
        public string? Message { get; set; }
        public string? MessageType { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Id = id;
            
            // Get message from TempData if any
            if (TempData.ContainsKey("Message"))
            {
                Message = TempData["Message"]?.ToString();
                MessageType = TempData["MessageType"]?.ToString();
            }
            
            using var client = CreateClient();
            
            // Get book details
            var response = await client.GetAsync($"https://localhost:5001/api/books/public/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Book = JsonSerializer.Deserialize<Book>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                // Check if user can borrow
                var userIdStr = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
                {
                    var borrowsResponse = await client.GetAsync($"https://localhost:5001/api/borrows/user/current/{userId}");
                    if (borrowsResponse.IsSuccessStatusCode)
                    {
                        var borrowsJson = await borrowsResponse.Content.ReadAsStringAsync();
                        var currentBorrows = JsonSerializer.Deserialize<List<BorrowRecord>>(borrowsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        CanBorrow = currentBorrows.Count < 5; // Max 5 books
                    }
                }
                
                return Page();
            }
            
            return RedirectToPage("/UserPage/Books");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                Message = "Vui lòng đăng nhập để mượn sách";
                MessageType = "danger";
                // Need to reload book data for proper display
                using var client = CreateClient();
                await LoadBookData(client, Id);
                return Page();
            }

            using var client2 = CreateClient();
            
            // Debug info
            System.Diagnostics.Debug.WriteLine($"BookDetail POST: UserId={userId}, BookId={Id}");
            
            // Create borrow record (same as debug page)
            var borrowData = new
            {
                UserId = userId,
                BookId = Id,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14)
            };
            
            var json = JsonSerializer.Serialize(borrowData);
            System.Diagnostics.Debug.WriteLine($"BookDetail JSON: {json}");
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client2.PostAsync("https://localhost:5001/api/borrows/user", content);
            
            System.Diagnostics.Debug.WriteLine($"BookDetail Response: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"BookDetail Success: {result}");
                
                // Set success message
                TempData["Message"] = "Mượn sách thành công!";
                TempData["MessageType"] = "success";
                
                // Redirect to current page to show message
                return RedirectToPage(new { id = Id });
            }
            else
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"BookDetail API Error: {response.StatusCode} - {errorJson}");
                
                try
                {
                    var errorObj = JsonSerializer.Deserialize<object>(errorJson);
                    Message = $"Mượn sách thất bại: {errorObj}";
                }
                catch
                {
                    Message = $"Mượn sách thất bại: {errorJson}";
                }
                MessageType = "danger";
                
                // Still need to reload book data for proper display
                await RefreshBookData(client2, userId);
            }
            
            return Page();
        }

        private async Task LoadBookData(HttpClient client, int bookId)
        {
            var bookResponse = await client.GetAsync($"https://localhost:5001/api/books/public/{bookId}");
            if (bookResponse.IsSuccessStatusCode)
            {
                var json = await bookResponse.Content.ReadAsStringAsync();
                Book = JsonSerializer.Deserialize<Book>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Book();
            }
        }

        private async Task RefreshBookData(HttpClient client, int userId)
        {
            // Refresh book data
            await LoadBookData(client, Id);

            // Refresh borrow status
            var borrowsResponse = await client.GetAsync($"https://localhost:5001/api/borrows/user/current/{userId}");
            if (borrowsResponse.IsSuccessStatusCode)
            {
                var borrowsJson = await borrowsResponse.Content.ReadAsStringAsync();
                var currentBorrows = JsonSerializer.Deserialize<List<BorrowRecord>>(borrowsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<BorrowRecord>();
                CanBorrow = currentBorrows.Count < 5; // Max 5 books
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