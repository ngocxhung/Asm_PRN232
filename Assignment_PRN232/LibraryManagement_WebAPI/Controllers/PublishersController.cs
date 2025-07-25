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
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherRepository _publisherRepository;
        public PublishersController(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _publisherRepository.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _publisherRepository.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Publisher publisher) => Ok(await _publisherRepository.CreateAsync(publisher));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Publisher publisher) => Ok(await _publisherRepository.UpdateAsync(id, publisher));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _publisherRepository.DeleteAsync(id));

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword) => Ok(await _publisherRepository.SearchAsync(keyword));
    }
} 