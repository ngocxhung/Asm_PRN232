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
    public class CategoriesModel : PageModel
    {
        public List<Category> Categories { get; set; }
        [BindProperty]
        public Category CategoryModel { get; set; }
        public bool ShowForm { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Admin")
                return RedirectToPage("/Login");
            Categories = await GetApi<List<Category>>("https://localhost:5001/api/categories");
            return Page();
        }

        public async Task<IActionResult> OnGetAddAsync()
        {
            ShowForm = true;
            CategoryModel = new Category();
            await LoadCategories();
            return Page();
        }

        public async Task<IActionResult> OnGetEditAsync(int id)
        {
            ShowForm = true;
            CategoryModel = await GetApi<Category>($"https://localhost:5001/api/categories/{id}");
            await LoadCategories();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            var isEdit = CategoryModel.CategoryId > 0;
            using var client = CreateClient();
            var json = JsonSerializer.Serialize(CategoryModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage res;
            if (isEdit)
                res = await client.PutAsync($"https://localhost:5001/api/categories/{CategoryModel.CategoryId}", content);
            else
                res = await client.PostAsync("https://localhost:5001/api/categories", content);
            if (!res.IsSuccessStatusCode)
            {
                var apiError = await res.Content.ReadAsStringAsync();
                ErrorMessage = !string.IsNullOrWhiteSpace(apiError) ? apiError : "Lưu thất bại!";
                ShowForm = true;
                await LoadCategories();
                return Page();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDeleteAsync(int id)
        {
            using var client = CreateClient();
            var res = await client.DeleteAsync($"https://localhost:5001/api/categories/{id}");
            if (!res.IsSuccessStatusCode)
                ErrorMessage = "Xóa thất bại!";
            await LoadCategories();
            return RedirectToPage();
        }

        private async Task LoadCategories()
        {
            Categories = await GetApi<List<Category>>("https://localhost:5001/api/categories");
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