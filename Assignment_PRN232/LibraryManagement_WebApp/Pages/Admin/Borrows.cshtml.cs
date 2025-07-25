using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Net.Http.Headers;
using BussinessObjects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement_WebApp.Pages.Admin
{
    public class BorrowsModel : PageModel
    {
        public List<BorrowRecord> Borrows { get; set; }
        public string ErrorMessage { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return RedirectToPage("/Login");
            using var client = CreateClient();
            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                var res = await client.GetAsync($"https://localhost:5001/api/borrows/search?keyword={SearchKeyword}");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    Borrows = JsonSerializer.Deserialize<List<BorrowRecord>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    Borrows = new List<BorrowRecord>();
                    ErrorMessage = "Không tìm được phiếu phù hợp!";
                }
            }
            else
            {
                var res = await client.GetAsync("https://localhost:5001/api/borrows");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    Borrows = JsonSerializer.Deserialize<List<BorrowRecord>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    Borrows = new List<BorrowRecord>();
                    ErrorMessage = "Không lấy được danh sách phiếu!";
                }
            }
            return Page();
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