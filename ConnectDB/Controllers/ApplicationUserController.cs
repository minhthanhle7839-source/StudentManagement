using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApplicationUserController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // 1. GET ALL
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // =========================
        // 2. GET BY ID
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        // =========================
        // 3. CREATE (REGISTER)
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // check trùng username/email
            var exists = await _context.Users
                .AnyAsync(u => u.Username == model.Username || u.Email == model.Email);

            if (exists)
                return BadRequest("Username hoặc Email đã tồn tại");

            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;

            // ⚠️ NOTE: chưa hash password (sẽ nâng cấp sau)
            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        // =========================
        // 4. UPDATE
        // =========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, ApplicationUser model)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found");

            user.Username = model.Username;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Avatar = model.Avatar;
            user.Country = model.Country;
            user.Status = model.Status;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // =========================
        // 5. DELETE
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }
}