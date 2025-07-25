using BussinessObjects.Models;
using DataAcess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class PublisherRepository : IPublisherRepository
    {
        private readonly PublisherDAO _publisherDao;
        public PublisherRepository(PublisherDAO publisherDao)
        {
            _publisherDao = publisherDao;
        }
        public Task<List<Publisher>> GetAllAsync() => _publisherDao.GetAllAsync();
        public Task<Publisher> GetByIdAsync(int id) => _publisherDao.GetByIdAsync(id);
        public Task<Publisher> CreateAsync(Publisher publisher) => _publisherDao.CreateAsync(publisher);
        public Task<Publisher> UpdateAsync(int id, Publisher publisher) => _publisherDao.UpdateAsync(id, publisher);
        public Task<bool> DeleteAsync(int id) => _publisherDao.DeleteAsync(id);
        public Task<List<Publisher>> SearchAsync(string keyword) => _publisherDao.SearchAsync(keyword);
    }
} 