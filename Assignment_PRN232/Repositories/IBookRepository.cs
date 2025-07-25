using BussinessObjects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllAsync();
        Task<Book> GetByIdAsync(int id);
        Task<Book> CreateAsync(Book book);
        Task<Book> UpdateAsync(int id, Book book);
        Task<bool> DeleteAsync(int id);
        Task<List<Book>> SearchAsync(string keyword);
        Task<List<Book>> FilterAsync(int? categoryId, int? authorId);
    }
} 