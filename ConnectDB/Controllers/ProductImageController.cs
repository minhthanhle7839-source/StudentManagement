using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/product-images")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductImageController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var images = await _context.ProductImages
                .Include(i => i.Product)
                .ToListAsync();

            return Ok(images);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var image = await _context.ProductImages
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
                return NotFound("Image not found");

            return Ok(image);
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create(ProductImage model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.CreatedAt = DateTime.UtcNow;

            _context.ProductImages.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, ProductImage model)
        {
            var image = await _context.ProductImages.FindAsync(id);

            if (image == null)
                return NotFound("Image not found");

            image.ImageUrl = model.ImageUrl;
            image.DisplayOrder = model.DisplayOrder;

            await _context.SaveChangesAsync();

            return Ok(image);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var image = await _context.ProductImages.FindAsync(id);

            if (image == null)
                return NotFound("Image not found");

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] long productId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File không hợp lệ");

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var image = new ProductImage
            {
                ProductId = productId,
                ImageUrl = "/uploads/images/" + fileName,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();

            return Ok(image);
        }
    }
}