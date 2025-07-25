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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _categoryRepository.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _categoryRepository.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category category)
        {
            try
            {
                return Ok(await _categoryRepository.CreateAsync(category));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category category)
        {
            try
            {
                return Ok(await _categoryRepository.UpdateAsync(id, category));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _categoryRepository.DeleteAsync(id));
    }
} 