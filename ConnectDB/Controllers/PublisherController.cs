using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PublisherController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/publisher
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Publishers.ToListAsync();
            return Ok(data);
        }

        // GET: api/publisher/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var pub = await _context.Publishers.FindAsync(id);
            if (pub == null) return NotFound();

            return Ok(pub);
        }

        // POST: api/publisher
        [HttpPost]
        public async Task<IActionResult> Create(Publisher model)
        {
            model.CreatedAt = DateTime.Now;

            _context.Publishers.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // PUT: api/publisher/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Publisher model)
        {
            var pub = await _context.Publishers.FindAsync(id);
            if (pub == null) return NotFound();

            pub.Name = model.Name;
            pub.Website = model.Website;

            await _context.SaveChangesAsync();

            return Ok(pub);
        }

        // DELETE: api/publisher/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var pub = await _context.Publishers.FindAsync(id);
            if (pub == null) return NotFound();

            _context.Publishers.Remove(pub);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }
}