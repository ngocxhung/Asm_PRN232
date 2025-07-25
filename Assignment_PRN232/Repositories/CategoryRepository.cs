using BussinessObjects.Models;
using DataAcess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _categoryDao;
        public CategoryRepository(CategoryDAO categoryDao)
        {
            _categoryDao = categoryDao;
        }
        public Task<List<Category>> GetAllAsync() => _categoryDao.GetAllAsync();
        public Task<Category> GetByIdAsync(int id) => _categoryDao.GetByIdAsync(id);
        public Task<Category> CreateAsync(Category category) => _categoryDao.CreateAsync(category);
        public Task<Category> UpdateAsync(int id, Category category) => _categoryDao.UpdateAsync(id, category);
        public Task<bool> DeleteAsync(int id) => _categoryDao.DeleteAsync(id);
    }
} 