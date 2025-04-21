using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HQSOFT_Ex1.Entities;
using HQSOFT_Ex1.DTOs;

namespace HQSOFT_Ex1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : Controller
    {
        private readonly SampleWebApiContext _context;

        public AuthorsController(SampleWebApiContext context)
        {
            _context = context;
        }

        // GET /api/authors/fetch
        [HttpGet("fetch")]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _context.Authors.ToListAsync();
            return Ok(authors);
        }

        // GET /api/authors/fetch/{AuthorId}
        [HttpGet("fetch/{id}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
                return NotFound();

            return Ok(author);
        }

        // POST /api/authors/insert
        [HttpPost("insert")]
        public async Task<IActionResult> CreateAuthor([FromBody] AuthorDto authorDto)
        {
            if (authorDto == null)
                return BadRequest("Invalid author data.");

            var author = new Author
            {
                AuthorId = authorDto.AuthorId,
                Name = authorDto.Name,
                Bio = authorDto.Bio
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT /api/authors/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAuthor([FromBody] AuthorDto authorDto)
        {
            if (authorDto == null)
                return BadRequest("Invalid author data.");

            var existingAuthor = await _context.Authors.FirstOrDefaultAsync(a => a.AuthorId == authorDto.AuthorId);
            if (existingAuthor == null)
                return NotFound();

            existingAuthor.Name = authorDto.Name;
            existingAuthor.Bio = authorDto.Bio;

            _context.Entry(existingAuthor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /api/authors/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
                return NotFound();

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
