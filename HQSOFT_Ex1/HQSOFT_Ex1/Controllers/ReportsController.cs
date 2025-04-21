using HQSOFT_Ex1.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HQSOFT_Ex1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : Controller
    {
        private readonly SampleWebApiContext _context;

        public ReportsController(SampleWebApiContext context)
        {
            _context = context;
        }

        // GET /api/reports/book
        [HttpGet("book")]
        public async Task<IActionResult> GetBooksByFilters(string searchKey = "", int? authorId = null, 
            DateTime? fromPublishedDate = null, DateTime? toPublishedDate = null, int pageSize = 10, int pageIndex = 1)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(b => b.Title.Contains(searchKey));
            }

            if (authorId.HasValue)
            {
                query = query.Where(b => b.AuthorId == authorId.Value);
            }

            if (fromPublishedDate.HasValue)
            {
                query = query.Where(b => b.PublishedDate >= fromPublishedDate.Value);
            }

            if (toPublishedDate.HasValue)
            {
                query = query.Where(b => b.PublishedDate <= toPublishedDate.Value);
            }

            // Phân trang
            var totalBooks = await query.CountAsync();
            var books = await query
                              .Skip((pageIndex - 1) * pageSize)
                              .Take(pageSize)
                              .Select(b => new ReportDto
                              {
                                  BookId = b.BookId,
                                  Title = b.Title,
                                  Price = b.Price,
                                  PublishedDate = b.PublishedDate,
                                  AuthorName = b.Author.Name
                              })
                          .ToListAsync();


            var result = new
            {
                TotalCount = totalBooks,
                PageSize = pageSize,
                PageIndex = pageIndex,
                Books = books
            };

            return Ok(result);
        }

    }
}
