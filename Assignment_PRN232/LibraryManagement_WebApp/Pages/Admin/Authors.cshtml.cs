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
    public class AuthorsModel : PageModel
    {
        public List<Author> Authors { get; set; }
        [BindProperty]
        public Author AuthorModel { get; set; }
        public bool ShowForm { get; set; }
        public string ErrorMessage { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchKeyword { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return RedirectToPage("/Login");
            if (!string.IsNullOrEmpty(SearchKeyword))
                Authors = await GetApi<List<Author>>($"https://localhost:5001/api/authors/search?keyword={SearchKeyword}");
            else
                Authors = await GetApi<List<Author>>("https://localhost:5001/api/authors");
            return Page();
        }

        public async Task<IActionResult> OnGetAddAsync()
        {
            ShowForm = true;
            AuthorModel = new Author();
            await LoadAuthors();
            return Page();
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            ShowForm = true;
            AuthorModel = await GetApi<Author>($"https://localhost:5001/api/authors/{id}");
            await LoadAuthors();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            var isEdit = AuthorModel.AuthorId > 0;
            using var client = CreateClient();
            var json = JsonSerializer.Serialize(AuthorModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage res;
            if (isEdit)
                res = await client.PutAsync($"https://localhost:5001/api/authors/{AuthorModel.AuthorId}", content);
            else
                res = await client.PostAsync("https://localhost:5001/api/authors", content);
            if (!res.IsSuccessStatusCode)
            {
                var apiError = await res.Content.ReadAsStringAsync();
                ErrorMessage = !string.IsNullOrWhiteSpace(apiError) ? apiError : "Lưu thất bại!";
                ShowForm = true;
                await LoadAuthors();
                return Page();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDeleteAsync(int id)
        {
            using var client = CreateClient();
            var res = await client.DeleteAsync($"https://localhost:5001/api/authors/{id}");
            if (!res.IsSuccessStatusCode)
                ErrorMessage = "Xóa thất bại!";
            await LoadAuthors();
            return RedirectToPage();
        }

        private async Task LoadAuthors()
        {
            Authors = await GetApi<List<Author>>("https://localhost:5001/api/authors");
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