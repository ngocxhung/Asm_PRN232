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
    public class BooksModel : PageModel
    {
        public List<Book> Books { get; set; }
        public List<Author> Authors { get; set; }
        public List<Category> Categories { get; set; }
        [BindProperty]
        public Book BookModel { get; set; }
        public bool ShowForm { get; set; }
        public string ErrorMessage { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? AuthorId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return RedirectToPage("/Login");
            await LoadDropdowns();
            if (!string.IsNullOrEmpty(SearchKeyword))
                Books = await GetApi<List<Book>>($"https://localhost:5001/api/books/search?keyword={SearchKeyword}");
            else if (CategoryId.HasValue || AuthorId.HasValue)
                Books = await GetApi<List<Book>>($"https://localhost:5001/api/books/filter?categoryId={CategoryId}&authorId={AuthorId}");
            else
                Books = await GetApi<List<Book>>("https://localhost:5001/api/books");
            return Page();
        }

        public async Task<IActionResult> OnGetAddAsync()
        {
            ShowForm = true;
            BookModel = new Book();
            await LoadDropdowns();
            await LoadBooks();
            return Page();
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            ShowForm = true;
            await LoadDropdowns();
            BookModel = await GetApi<Book>($"https://localhost:5001/api/books/{id}");
            await LoadBooks();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            var isEdit = BookModel.BookId > 0;
            using var client = CreateClient();
            var json = JsonSerializer.Serialize(BookModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage res;
            if (isEdit)
                res = await client.PutAsync($"https://localhost:5001/api/books/{BookModel.BookId}", content);
            else
                res = await client.PostAsync("https://localhost:5001/api/books", content);
            if (!res.IsSuccessStatusCode)
            {
                ErrorMessage = "Lưu thất bại!";
                ShowForm = true;
                await LoadDropdowns();
                await LoadBooks();
                return Page();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDeleteAsync(int id)
        {
            using var client = CreateClient();
            var res = await client.DeleteAsync($"https://localhost:5001/api/books/{id}");
            if (!res.IsSuccessStatusCode)
                ErrorMessage = "Xóa thất bại!";
            await LoadBooks();
            return RedirectToPage();
        }

        private async Task LoadBooks()
        {
            Books = await GetApi<List<Book>>("https://localhost:5001/api/books");
        }
        private async Task LoadDropdowns()
        {
            Authors = await GetApi<List<Author>>("https://localhost:5001/api/books/authors");
            Categories = await GetApi<List<Category>>("https://localhost:5001/api/books/categories");
        }
        private async Task<T> GetApi<T>(string url)
        {
            using var client = CreateClient();
            var res = await client.GetAsync(url);
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return default;
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