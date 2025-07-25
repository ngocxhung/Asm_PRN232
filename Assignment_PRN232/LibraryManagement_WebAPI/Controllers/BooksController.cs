using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using BussinessObjects.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly LibraryDbContext _context;
        public BooksController(IBookRepository bookRepository, LibraryDbContext context)
        {
            _bookRepository = bookRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _bookRepository.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _bookRepository.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book book) => Ok(await _bookRepository.CreateAsync(book));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book book) => Ok(await _bookRepository.UpdateAsync(id, book));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _bookRepository.DeleteAsync(id));

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword) => Ok(await _bookRepository.SearchAsync(keyword));

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] int? categoryId, [FromQuery] int? authorId) => Ok(await _bookRepository.FilterAsync(categoryId, authorId));

        [HttpGet("authors")]
        public async Task<IActionResult> GetAuthors() => Ok(await _context.Authors.ToListAsync());

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories() => Ok(await _context.Categories.ToListAsync());
    }
} 