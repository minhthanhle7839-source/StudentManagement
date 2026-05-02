// Controllers/LibraryController.cs
using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/library")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LibraryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/library/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(long userId)
        {
            var library = await _context.Libraries
                .Include(l => l.Product)
                    .ThenInclude(p => p!.Images)
                .Include(l => l.Product)
                    .ThenInclude(p => p!.Versions)
                .Where(l => l.UserId == userId && l.Status == 1)
                .ToListAsync();

            return Ok(library);
        }

        // GET: api/library/check?userId=1&productId=2
        // Dùng ở product detail để check user đã mua chưa
        [HttpGet("check")]
        public async Task<IActionResult> Check([FromQuery] long userId, [FromQuery] long productId)
        {
            var owned = await _context.Libraries
                .AnyAsync(l => l.UserId == userId && l.ProductId == productId && l.Status == 1);
            return Ok(new { owned });
        }

        // GET: api/library/download/{versionId}?userId=1
        // Tải file — chỉ cho phép nếu user đã sở hữu game
        // GET: api/library/download/{versionId}?userId=1
        [HttpGet("download/{versionId}")]
        public async Task<IActionResult> Download(long versionId, [FromQuery] long userId)
        {
            var version = await _context.ProductVersions.FindAsync(versionId);
            if (version == null) return NotFound("Version not found");

            // Kiểm tra quyền sở hữu
            var owned = await _context.Libraries
                .AnyAsync(l => l.UserId == userId && l.ProductId == version.ProductId && l.Status == 1);

            if (!owned) return Forbid();

            // FileUrl dạng "/files/abc.zip" → map sang wwwroot/files/abc.zip
            var filePath = Path.Combine(_env.WebRootPath, version.FileUrl.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound("File không tồn tại trên server");

            var fileName = Path.GetFileName(filePath);
            return PhysicalFile(filePath, "application/octet-stream", fileName);
        }
        // GET: api/library/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var lib = await _context.Libraries
                .Include(l => l.Product)
                .FirstOrDefaultAsync(l => l.Id == id);
            if (lib == null) return NotFound();
            return Ok(lib);
        }
    }
}