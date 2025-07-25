using BussinessObjects.Models;
using DataAcess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookDAO _bookDao;
        public BookRepository(BookDAO bookDao)
        {
            _bookDao = bookDao;
        }

        public BookRepository()
        {
        }

        public Task<List<Book>> GetAllAsync() => _bookDao.GetAllAsync();
        public Task<Book> GetByIdAsync(int id) => _bookDao.GetByIdAsync(id);
        public Task<Book> CreateAsync(Book book) => _bookDao.CreateAsync(book);
        public Task<Book> UpdateAsync(int id, Book book) => _bookDao.UpdateAsync(id, book);
        public Task<bool> DeleteAsync(int id) => _bookDao.DeleteAsync(id);
        public Task<List<Book>> SearchAsync(string keyword) => _bookDao.SearchAsync(keyword);
        public Task<List<Book>> FilterAsync(int? categoryId, int? authorId) => _bookDao.FilterAsync(categoryId, authorId);
    }
} 