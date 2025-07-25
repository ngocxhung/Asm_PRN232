using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System;

namespace LibraryManagement_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinesController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        public FinesController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Fines
                .Include(f => f.BorrowRecord)
                .ThenInclude(br => br.User)
                .ToListAsync();
            return new JsonResult(data, new System.Text.Json.JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var query = _context.Fines
                .Include(f => f.BorrowRecord)
                .ThenInclude(br => br.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(f => f.FineId.ToString() == keyword || 
                                        (f.BorrowRecord != null && f.BorrowRecord.User != null && 
                                         f.BorrowRecord.User.FullName.Contains(keyword)));
            }

            var data = await query.ToListAsync();
            return new JsonResult(data, new System.Text.Json.JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            });
        }

        // User endpoints
        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserFines(int userId)
        {
            var fines = await _context.Fines
                .Include(f => f.BorrowRecord)
                .ThenInclude(br => br.Book)
                .ThenInclude(book => book.Author)
                .Where(f => f.BorrowRecord.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return new JsonResult(fines, new System.Text.Json.JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            });
        }

        [HttpPost("{fineId}/pay")]
        [AllowAnonymous]
        public async Task<IActionResult> PayFine(int fineId)
        {
            try
            {
                var fine = await _context.Fines
                    .Include(f => f.BorrowRecord)
                    .FirstOrDefaultAsync(f => f.FineId == fineId);

                if (fine == null)
                    return NotFound("Khong tim thay phat");

                if (fine.Status == "Paid")
                    return BadRequest("Phat nay da duoc thanh toan");

                // Update fine status
                fine.Status = "Paid";
                fine.PaidAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Thanh toan thanh cong", fine });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
    }
} 