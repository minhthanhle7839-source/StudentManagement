using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // 1. GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.Developer)
                .Include(p => p.Publisher)
                .ToListAsync();

            return Ok(products);
        }

        // =========================
        // 2. GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var product = await _context.Products
                .Include(p => p.Developer)
                .Include(p => p.Publisher)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }

        // =========================
        // 3. CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(Product model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.Slug = GenerateSlug(model.Name);
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;

            _context.Products.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // =========================
        // 4. UPDATE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Product model)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound("Product not found");

            // update fields
            product.Name = model.Name;
            product.Slug = GenerateSlug(model.Name);
            product.Description = model.Description;
            product.Price = model.Price;
            product.DeveloperId = model.DeveloperId;
            product.PublisherId = model.PublisherId;
            product.ReleaseDate = model.ReleaseDate;
            product.Status = model.Status;
            product.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // =========================
        // 5. DELETE (HARD DELETE)
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound("Product not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }

        // =========================
        // 6. SOFT DELETE (OPTIONAL)
        // =========================
        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SoftDelete(long id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound("Product not found");

            product.Status = 0; // 0 = inactive
            product.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok("Soft deleted");
        }

        // =========================
        // HELPER: SLUG
        // =========================
        private string GenerateSlug(string input)
        {
            string slug = input.ToLower();
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-").Trim('-');
            return slug;
        }
    }
}