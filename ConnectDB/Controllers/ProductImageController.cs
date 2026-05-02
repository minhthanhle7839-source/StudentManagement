using ConnectDB.Data;
using ConnectDB.DTO;
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file");

            var ext = Path.GetExtension(request.File.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine("wwwroot/images", fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await request.File.CopyToAsync(stream);

            var image = new ProductImage
            {
                ProductId = request.ProductId,
                ImageUrl = $"/images/{fileName}"
            };

            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();

            return Ok(image);
        }
    }
}