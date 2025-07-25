using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using BussinessObjects.Models;
using System.Threading.Tasks;

namespace LibraryManagement_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _userRepository.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _userRepository.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user) => Ok(await _userRepository.CreateAsync(user));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User user) => Ok(await _userRepository.UpdateAsync(id, user));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _userRepository.DeleteAsync(id));
    }
} 