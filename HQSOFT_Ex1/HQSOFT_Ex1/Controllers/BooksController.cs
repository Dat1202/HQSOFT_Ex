using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HQSOFT_Ex1.Entities;
using HQSOFT_Ex1.DTOs;

namespace HQSOFT_Ex1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private readonly SampleWebApiContext _context;

        public BooksController(SampleWebApiContext context)
        {
            _context = context;
        }

        // GET /api/books/fetch 
        [HttpGet("fetch")]
        public async Task<ActionResult> GetAllBooks()
        {
            var books = await _context.Books.ToListAsync();
            return Ok(books);
        }

        // GET /api/books/fetch/{BookId} 
        [HttpGet("fetch/{id}")]
        public async Task<ActionResult<BookDto>> GetBookById(int id)
        {
            var book = await _context.Books.Include(b => b.Author)
                .Where(b => b.BookId == id)
                .Select(b => new BookDto
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Price = b.Price,
                    PublishedDate = b.PublishedDate
                }).FirstOrDefaultAsync();

            if (book == null) return NotFound();
            return Ok(book);
        }

        // POST /api/books/insert
        [HttpPost("insert")]
        public async Task<IActionResult> CreateBook([FromBody] BookDto bookDto)
        {
            if (bookDto == null)
                return BadRequest("Invalid book data.");

            var book = new Book
            {
                Title = bookDto.Title,
                Price = bookDto.Price,
                PublishedDate = bookDto.PublishedDate,
                AuthorId = bookDto.AuthorId,
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return Ok();
        }


        // PUT /api/books/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateBook([FromBody] BookDto bookDto)
        {
            if (bookDto == null)
                return BadRequest("Invalid book data.");

            // Kiểm tra xem AuthorId có hợp lệ không
            var existingAuthor = await _context.Authors.FindAsync(bookDto.AuthorId);
            if (existingAuthor == null)
                return BadRequest("Invalid AuthorId.");

            var existingBook = await _context.Books.FindAsync(bookDto.BookId);
            if (existingBook == null)
                return NotFound();

            existingBook.Title = bookDto.Title;
            existingBook.Price = bookDto.Price;
            existingBook.PublishedDate = bookDto.PublishedDate;
            existingBook.AuthorId = bookDto.AuthorId;

            _context.Entry(existingAuthor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /api/books/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }
    }
}
