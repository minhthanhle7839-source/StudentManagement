using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeveloperController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DeveloperController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/developer
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Developers.ToListAsync();
            return Ok(data);
        }

        // GET: api/developer/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var dev = await _context.Developers.FindAsync(id);
            if (dev == null) return NotFound();

            return Ok(dev);
        }

        // POST: api/developer
        [HttpPost]
        public async Task<IActionResult> Create(Developer model)
        {
            model.CreatedAt = DateTime.UtcNow;

            _context.Developers.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // PUT: api/developer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Developer model)
        {
            var dev = await _context.Developers.FindAsync(id);
            if (dev == null) return NotFound();

            dev.Name = model.Name;
            dev.Description = model.Description;
            dev.Website = model.Website;

            await _context.SaveChangesAsync();

            return Ok(dev);
        }

        // DELETE: api/developer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var dev = await _context.Developers.FindAsync(id);
            if (dev == null) return NotFound();

            _context.Developers.Remove(dev);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }
}