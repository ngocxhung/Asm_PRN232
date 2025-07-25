using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BussinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagement_WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class BorrowsController : ControllerBase
    {
        private readonly LibraryDbContext _context;
        public BorrowsController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<BorrowRecord>>> GetAll()
        {
            return await _context.BorrowRecords.Include(b => b.User).ToListAsync();
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<BorrowRecord>>> Search([FromQuery] string keyword)
        {
            var query = _context.BorrowRecords.Include(b => b.User).AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(b => b.BorrowId.ToString() == keyword || (b.User != null && b.User.FullName.Contains(keyword)));
            }
            return await query.ToListAsync();
        }
    }
} 