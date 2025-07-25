using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAcess
{
    public class AuthorDAO
    {
        private readonly LibraryDbContext _context;
        public AuthorDAO(LibraryDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<List<Author>> GetAllAsync() => await _context.Authors.ToListAsync();
        public async Task<Author> GetByIdAsync(int id) => await _context.Authors.FindAsync(id);
        public async Task<Author> CreateAsync(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }
        public async Task<Author> UpdateAsync(int id, Author author)
        {
            var existing = await _context.Authors.FindAsync(id);
            if (existing == null) return null;
            existing.AuthorName = author.AuthorName;
            existing.Description = author.Description;
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return false;
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Author>> SearchAsync(string keyword)
        {
            return await _context.Authors.Where(a => a.AuthorName.Contains(keyword)).ToListAsync();
        }
    }
} 