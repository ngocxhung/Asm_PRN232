using BussinessObjects.Models;
using DataAcess;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO _userDao;
        public UserRepository(UserDAO userDao)
        {
            _userDao = userDao;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userDao.GetByUsernameAsync(username);
            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;
            return user;
        }

        public Task<List<User>> GetAllAsync() => _userDao.GetAllAsync();
        public Task<User> GetByIdAsync(int id) => _userDao.GetByIdAsync(id);
        public Task<User> CreateAsync(User user) => _userDao.CreateAsync(user);
        public Task<User> UpdateAsync(int id, User user) => _userDao.UpdateAsync(id, user);
        public Task<bool> DeleteAsync(int id) => _userDao.DeleteAsync(id);
    }
}
