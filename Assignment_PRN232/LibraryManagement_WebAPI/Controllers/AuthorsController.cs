using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using BussinessObjects.Models;
using System.Threading.Tasks;

namespace LibraryManagement_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() => Ok(await _authorRepository.GetAllAsync());

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id) => Ok(await _authorRepository.GetByIdAsync(id));

        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id) => Ok(await _authorRepository.DeleteAsync(id));

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string keyword) => Ok(await _authorRepository.SearchAsync(keyword));
    }
} 