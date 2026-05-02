using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private readonly Cloudinary _cloudinary;

        public ProductImageController(AppDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
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
            if (image == null) return NotFound("Image not found");
            return Ok(image);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var image = await _context.ProductImages.FindAsync(id);
            if (image == null) return NotFound("Image not found");
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            return Ok("Deleted successfully");
        }

        // UPLOAD → Cloudinary
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file");

            // Upload lên Cloudinary
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(request.File.FileName, request.File.OpenReadStream()),
                Folder = "gamestore/products",
               
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                return BadRequest(uploadResult.Error.Message);

            // Lưu URL Cloudinary vào DB
            var image = new ProductImage
            {
                ProductId = request.ProductId,
                ImageUrl = uploadResult.SecureUrl.ToString(), // https://res.cloudinary.com/...
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();
            return Ok(image);
        }
    }
}