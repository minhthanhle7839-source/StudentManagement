using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
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
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Images)
                .Include(p => p.Versions)
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
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Images)     
                .Include(p => p.Versions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }

        // =========================
        // 3. CREATE (JSON có categoryIds)
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(Product model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.Slug = GenerateSlug(model.Name);
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;

            // lưu product trước
            _context.Products.Add(model);
            await _context.SaveChangesAsync();

            // thêm category
            if (model.CategoryIds != null && model.CategoryIds.Any())
            {
                var relations = model.CategoryIds.Select(cid => new ProductCategory
                {
                    ProductId = model.Id,
                    CategoryId = cid
                });

                _context.ProductCategories.AddRange(relations);
                await _context.SaveChangesAsync();
            }

            return Ok(model);
        }
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var product = await _context.Products
                .Include(p => p.Developer)
                .Include(p => p.Publisher)
                .Include(p => p.ProductCategories).ThenInclude(pc => pc.Category)
                .Include(p => p.Images)
                .Include(p => p.Versions)
                .FirstOrDefaultAsync(p => p.Slug == slug);

            if (product == null) return NotFound("Product not found");
            return Ok(product);
        }
        // =========================
        // 4. UPDATE (sync categoryIds)
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, Product model)
        {
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound("Product not found");

            // update field
            product.Name = model.Name;
            product.Slug = GenerateSlug(model.Name);
            product.Description = model.Description;
            product.Price = model.Price;
            product.DeveloperId = model.DeveloperId;
            product.PublisherId = model.PublisherId;
            product.ReleaseDate = model.ReleaseDate;
            product.Status = model.Status;
            product.UpdatedAt = DateTime.UtcNow;

            // xử lý category
            if (model.CategoryIds != null)
            {
                // xóa cũ
                _context.ProductCategories.RemoveRange(product.ProductCategories);

                // thêm mới
                var relations = model.CategoryIds.Select(cid => new ProductCategory
                {
                    ProductId = product.Id,
                    CategoryId = cid
                });

                _context.ProductCategories.AddRange(relations);
            }

            await _context.SaveChangesAsync();

            return Ok(product);
        }

        // =========================
        // 5. DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound("Product not found");

            // xóa bảng trung gian
            var relations = _context.ProductCategories
                .Where(pc => pc.ProductId == id);

            _context.ProductCategories.RemoveRange(relations);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
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