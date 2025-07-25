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
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _authorRepository.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _authorRepository.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Author author)
        {
            try
            {
                return Ok(await _authorRepository.CreateAsync(author));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Author author)
        {
            try
            {
                return Ok(await _authorRepository.UpdateAsync(id, author));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _authorRepository.DeleteAsync(id));

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword) => Ok(await _authorRepository.SearchAsync(keyword));
    }
} 