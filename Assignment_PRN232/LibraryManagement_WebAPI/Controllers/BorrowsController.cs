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

        // User endpoints
        [HttpGet("user/current/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<BorrowRecord>>> GetUserCurrentBorrows(int userId)
        {
            var borrows = await _context.BorrowRecords
                .Include(b => b.User)
                .Include(b => b.Book)
                .ThenInclude(book => book.Author)
                .Include(b => b.Book)
                .ThenInclude(book => book.Category)
                .Where(b => b.UserId == userId && b.Status == "Borrowed")
                .ToListAsync();
            return Ok(borrows);
        }

        [HttpGet("user/history/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<BorrowRecord>>> GetUserBorrowHistory(int userId)
        {
            var borrows = await _context.BorrowRecords
                .Include(b => b.User)
                .Include(b => b.Book)
                .ThenInclude(book => book.Author)
                .Include(b => b.Book)
                .ThenInclude(book => book.Category)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();
            return Ok(borrows);
        }

        [HttpPost("user")]
        [AllowAnonymous]
        public async Task<IActionResult> UserBorrowBook([FromBody] BorrowRequest request)
        {
            try
            {
                // Check if user exists
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                    return BadRequest("User khong ton tai");

                // Check if book exists and is available
                var book = await _context.Books.FindAsync(request.BookId);
                if (book == null)
                    return BadRequest("Sach khong ton tai");
                if (book.Available <= 0)
                    return BadRequest("Sach khong co san");

                // Check if user has unpaid fines
                var unpaidFines = await _context.Fines
                    .Include(f => f.BorrowRecord)
                    .Where(f => f.BorrowRecord.UserId == request.UserId && f.Status == "Unpaid")
                    .AnyAsync();
                if (unpaidFines)
                    return BadRequest("Ban co phat chua thanh toan, khong the muon sach");

                // Check if user has reached borrow limit (5 books)
                var currentBorrows = await _context.BorrowRecords
                    .Where(b => b.UserId == request.UserId && b.Status == "Borrowed")
                    .CountAsync();
                if (currentBorrows >= 5)
                    return BadRequest("Ban da muon toi da 5 sach");

                // Create borrow record
                var borrowRecord = new BorrowRecord
                {
                    UserId = request.UserId,
                    BookId = request.BookId,
                    BorrowDate = request.BorrowDate,
                    DueDate = request.DueDate,
                    Status = "Borrowed",
                    ExtendCount = 0,
                    Note = string.Empty
                };

                _context.BorrowRecords.Add(borrowRecord);

                // Update book availability
                book.Available--;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Muon sach thanh cong", borrowRecord });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

        [HttpPut("{borrowId}/extend")]
        [AllowAnonymous]
        public async Task<IActionResult> ExtendBorrow(int borrowId)
        {
            try
            {
                var borrowRecord = await _context.BorrowRecords
                    .Include(b => b.Book)
                    .FirstOrDefaultAsync(b => b.BorrowId == borrowId);

                if (borrowRecord == null)
                    return NotFound("Không tìm thấy record mượn sách");

                if (borrowRecord.Status != "Borrowed")
                    return BadRequest("Sach nay khong trong trang thai muon");

                if (borrowRecord.ExtendCount >= 2)
                    return BadRequest("Ban da gia han toi da 2 lan cho sach nay");

                // Extend due date by 7 days
                borrowRecord.DueDate = borrowRecord.DueDate.AddDays(7);
                borrowRecord.ExtendCount++;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Gia han thanh cong", borrowRecord });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }

        [HttpPut("{borrowId}/return")]
        [AllowAnonymous]
        public async Task<IActionResult> ReturnBook(int borrowId)
        {
            try
            {
                var borrowRecord = await _context.BorrowRecords
                    .Include(b => b.Book)
                    .FirstOrDefaultAsync(b => b.BorrowId == borrowId);

                if (borrowRecord == null)
                    return NotFound("Không tìm thấy record mượn sách");

                if (borrowRecord.Status != "Borrowed")
                    return BadRequest("Sach nay khong trong trang thai muon");

                // Update borrow record
                borrowRecord.Status = "Returned";
                borrowRecord.ReturnDate = DateTime.Now;

                // Update book availability
                borrowRecord.Book.Available++;

                // Check for overdue and create fine if necessary
                if (borrowRecord.DueDate < DateTime.Now)
                {
                    var daysLate = (DateTime.Now - borrowRecord.DueDate).Days;
                    var fineAmount = daysLate * 5000; // 5000 VND per day

                    var fine = new Fine
                    {
                        BorrowId = borrowRecord.BorrowId,
                        Amount = fineAmount,
                        DaysLate = daysLate,
                        Status = "Unpaid",
                        CreatedAt = DateTime.Now
                    };

                    _context.Fines.Add(fine);
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Tra sach thanh cong", borrowRecord });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
    }

    public class BorrowRequest
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
    }
} 