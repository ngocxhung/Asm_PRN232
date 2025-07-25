using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAcess
{
    public class CategoryDAO
    {
        private readonly LibraryDbContext _context;
        public CategoryDAO(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetAllAsync() => await _context.Categories.ToListAsync();
        public async Task<Category> GetByIdAsync(int id) => await _context.Categories.FindAsync(id);
        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }
        public async Task<Category> UpdateAsync(int id, Category category)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return null;
            existing.CategoryName = category.CategoryName;
            existing.Description = category.Description;
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 