using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/product-versions")]
    [ApiController]
    public class ProductVersionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductVersionController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var versions = await _context.ProductVersions
                .Include(v => v.Product)
                .ToListAsync();

            return Ok(versions);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var version = await _context.ProductVersions
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (version == null)
                return NotFound("Version not found");

            return Ok(version);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(ProductVersion model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.CreatedAt = DateTime.UtcNow;

            _context.ProductVersions.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, ProductVersion model)
        {
            var version = await _context.ProductVersions.FindAsync(id);

            if (version == null)
                return NotFound("Version not found");

            version.Version = model.Version;
            version.Changelog = model.Changelog;
            version.ReleaseDate = model.ReleaseDate;
            version.FileSize = model.FileSize;

            await _context.SaveChangesAsync();

            return Ok(version);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var version = await _context.ProductVersions.FindAsync(id);

            if (version == null)
                return NotFound("Version not found");

            _context.ProductVersions.Remove(version);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }
}