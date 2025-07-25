using BussinessObjects.Models;
using DataAcess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly AuthorDAO _authorDao;
        public AuthorRepository(AuthorDAO authorDao)
        {
            _authorDao = authorDao;
        }
        public Task<List<Author>> GetAllAsync() => _authorDao.GetAllAsync();
        public Task<Author> GetByIdAsync(int id) => _authorDao.GetByIdAsync(id);
        public Task<Author> CreateAsync(Author author) => _authorDao.CreateAsync(author);
        public Task<Author> UpdateAsync(int id, Author author) => _authorDao.UpdateAsync(id, author);
        public Task<bool> DeleteAsync(int id) => _authorDao.DeleteAsync(id);
        public Task<List<Author>> SearchAsync(string keyword) => _authorDao.SearchAsync(keyword);
    }
} 