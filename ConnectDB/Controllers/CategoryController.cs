using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // 1. GET ALL (kèm product)
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Include(c => c.ProductCategories)
                    .ThenInclude(pc => pc.Product)
                .ToListAsync();

            return Ok(categories);
        }

        // =========================
        // 2. GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var category = await _context.Categories
                .Include(c => c.ProductCategories)
                    .ThenInclude(pc => pc.Product)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound("Category not found");

            return Ok(category);
        }

        // =========================
        // 3. CREATE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.Slug = GenerateSlug(model.Name);
            model.CreatedAt = DateTime.UtcNow;

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // =========================
        // 4. UPDATE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Category model)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound("Category not found");

            category.Name = model.Name;
            category.Slug = GenerateSlug(model.Name);
            category.Description = model.Description;

            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // =========================
        // 5. DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound("Category not found");

            // xóa liên kết trước
            var relations = _context.ProductCategories.Where(pc => pc.CategoryId == id);
            _context.ProductCategories.RemoveRange(relations);

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }

        // =========================
        // HELPER
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