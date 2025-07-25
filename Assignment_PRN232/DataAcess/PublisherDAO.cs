using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAcess
{
    public class PublisherDAO
    {
        private readonly LibraryDbContext _context;
        public PublisherDAO(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<List<Publisher>> GetAllAsync() => await _context.Publishers.ToListAsync();
        public async Task<Publisher> GetByIdAsync(int id) => await _context.Publishers.FindAsync(id);
        public async Task<Publisher> CreateAsync(Publisher publisher)
        {
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();
            return publisher;
        }
        public async Task<Publisher> UpdateAsync(int id, Publisher publisher)
        {
            var existing = await _context.Publishers.FindAsync(id);
            if (existing == null) return null;
            existing.PublisherName = publisher.PublisherName;
            existing.ContactInfo = publisher.ContactInfo;
            await _context.SaveChangesAsync();
            return existing;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return false;
            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Publisher>> SearchAsync(string keyword)
        {
            return await _context.Publishers.Where(p => p.PublisherName.Contains(keyword)).ToListAsync();
        }
    }
} 