using BussinessObjects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IPublisherRepository
    {
        Task<List<Publisher>> GetAllAsync();
        Task<Publisher> GetByIdAsync(int id);
        Task<Publisher> CreateAsync(Publisher publisher);
        Task<Publisher> UpdateAsync(int id, Publisher publisher);
        Task<bool> DeleteAsync(int id);
        Task<List<Publisher>> SearchAsync(string keyword);
    }
} 