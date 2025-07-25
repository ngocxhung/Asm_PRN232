using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace LibraryManagement_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
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
                query = query.Where(f => f.FineId.ToString() == keyword || (f.BorrowRecord != null && f.BorrowRecord.User != null && f.BorrowRecord.User.FullName.Contains(keyword)));
            }
            var data = await query.ToListAsync();
            return new JsonResult(data, new System.Text.Json.JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
            });
        }
    }
} 