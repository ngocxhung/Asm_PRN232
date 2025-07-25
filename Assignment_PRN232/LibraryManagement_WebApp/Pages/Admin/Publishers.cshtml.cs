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
    public class PublishersModel : PageModel
    {
        public List<Publisher> Publishers { get; set; }
        [BindProperty]
        public Publisher PublisherModel { get; set; }
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
                Publishers = await GetApi<List<Publisher>>($"https://localhost:5001/api/publishers/search?keyword={SearchKeyword}");
            else
                Publishers = await GetApi<List<Publisher>>("https://localhost:5001/api/publishers");
            return Page();
        }

        public async Task<IActionResult> OnGetAddAsync()
        {
            ShowForm = true;
            PublisherModel = new Publisher();
            await LoadPublishers();
            return Page();
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            ShowForm = true;
            PublisherModel = await GetApi<Publisher>($"https://localhost:5001/api/publishers/{id}");
            await LoadPublishers();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            var isEdit = PublisherModel.PublisherId > 0;
            using var client = CreateClient();
            var json = JsonSerializer.Serialize(PublisherModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage res;
            if (isEdit)
                res = await client.PutAsync($"https://localhost:5001/api/publishers/{PublisherModel.PublisherId}", content);
            else
                res = await client.PostAsync("https://localhost:5001/api/publishers", content);
            if (!res.IsSuccessStatusCode)
            {
                ErrorMessage = "Lưu thất bại!";
                ShowForm = true;
                await LoadPublishers();
                return Page();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDeleteAsync(int id)
        {
            using var client = CreateClient();
            var res = await client.DeleteAsync($"https://localhost:5001/api/publishers/{id}");
            if (!res.IsSuccessStatusCode)
                ErrorMessage = "Xóa thất bại!";
            await LoadPublishers();
            return RedirectToPage();
        }

        private async Task LoadPublishers()
        {
            Publishers = await GetApi<List<Publisher>>("https://localhost:5001/api/publishers");
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