using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using BussinessObjects.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LibraryManagement_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly LibraryDbContext _context;
        public BooksController(IBookRepository bookRepository, LibraryDbContext context)
        {
            _bookRepository = bookRepository;
            _context = context;
        }
            [Authorize(Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _bookRepository.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _bookRepository.GetByIdAsync(id));
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Book book)
        {
            try
            {
                return Ok(await _bookRepository.CreateAsync(book));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Book book)
        {
            try
            {
                return Ok(await _bookRepository.UpdateAsync(id, book));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _bookRepository.DeleteAsync(id));

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword) => Ok(await _bookRepository.SearchAsync(keyword));

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] int? categoryId, [FromQuery] int? authorId) => Ok(await _bookRepository.FilterAsync(categoryId, authorId));

        // Public endpoints for users
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicBooks()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Where(b => b.Status == "Available")
                .ToListAsync();
            return Ok(books);
        }

        [HttpGet("public/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.BookId == id);
            
            if (book == null)
                return NotFound();
                
            return Ok(book);
        }

        [HttpGet("public/filter")]
        [AllowAnonymous]
        public async Task<IActionResult> FilterPublicBooks([FromQuery] string keyword, [FromQuery] int? categoryId, [FromQuery] int? authorId)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Where(b => b.Status == "Available");

            if (!string.IsNullOrEmpty(keyword))
            {
                // Case-insensitive search across multiple fields
                query = query.Where(b => 
                    b.Title.ToLower().Contains(keyword.ToLower()) || 
                    b.Author.AuthorName.ToLower().Contains(keyword.ToLower()) ||
                    (b.Description != null && b.Description.ToLower().Contains(keyword.ToLower())) ||
                    (b.ISBN != null && b.ISBN.Contains(keyword)) ||
                    b.Category.CategoryName.ToLower().Contains(keyword.ToLower())
                );
            }

            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == categoryId);
            }

            if (authorId.HasValue)
            {
                query = query.Where(b => b.AuthorId == authorId);
            }

            var books = await query.ToListAsync();
            return Ok(books);
        }
    }
} 