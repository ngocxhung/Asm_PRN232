using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAcess
{
    public class BookDAO
    {
        private readonly LibraryDbContext _context;
        public BookDAO(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllAsync() => await _context.Books.Include(b => b.Author).Include(b => b.Category).ToListAsync();
        public async Task<Book> GetByIdAsync(int id) => await _context.Books.Include(b => b.Author).Include(b => b.Category).FirstOrDefaultAsync(b => b.BookId == id);
        public async Task<Book> CreateAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }
        public async Task<Book> UpdateAsync(int id, Book book)
        {
            var existing = await _context.Books.FindAsync(id);
            if (existing == null) return null;
            existing.Title = book.Title;
            existing.AuthorId = book.AuthorId;
            existing.CategoryId = book.CategoryId;
            existing.PublisherId = book.PublisherId;
            existing.PublishYear = book.PublishYear;
            existing.ISBN = book.ISBN;
            existing.Description = book.Description;
            existing.Quantity = book.Quantity;
            existing.Available = book.Available;
            existing.Location = book.Location;
            existing.Status = book.Status;
            existing.ImageUrl = book.ImageUrl;
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Book>> SearchAsync(string keyword)
        {
            return await _context.Books.Include(b => b.Author).Include(b => b.Category)
                .Where(b => b.Title.Contains(keyword) || b.BookId.ToString() == keyword)
                .ToListAsync();
        }
        public async Task<List<Book>> FilterAsync(int? categoryId, int? authorId)
        {
            var query = _context.Books.Include(b => b.Author).Include(b => b.Category).AsQueryable();
            if (categoryId.HasValue)
                query = query.Where(b => b.CategoryId == categoryId);
            if (authorId.HasValue)
                query = query.Where(b => b.AuthorId == authorId);
            return await query.ToListAsync();
        }
    }
} 