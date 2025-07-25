using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using BussinessObjects.Models;

namespace LibraryManagement_WebApp.Pages.UserPage
{
    public class BooksModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; } = string.Empty;
        
        public List<Book> Books { get; set; } = new List<Book>();

        public async Task OnGetAsync()
        {
            using var client = CreateClient();
            
            string url;
            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                url = $"https://localhost:5001/api/books/public/filter?keyword={Uri.EscapeDataString(SearchKeyword)}";
            }
            else
            {
                url = "https://localhost:5001/api/books/public";
            }
            
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"Books search - URL: {url}");
            System.Diagnostics.Debug.WriteLine($"Search keyword: '{SearchKeyword}'");
            
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Books = JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Book>();
                
                System.Diagnostics.Debug.WriteLine($"Books search - Found {Books.Count} books");
            }
            else
            {
                // Log error for debugging
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Books API failed: {response.StatusCode}, URL: {url}");
                System.Diagnostics.Debug.WriteLine($"Error content: {errorContent}");
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