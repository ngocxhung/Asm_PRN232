using BussinessObjects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(int id, Category category);
        Task<bool> DeleteAsync(int id);
    }
} 