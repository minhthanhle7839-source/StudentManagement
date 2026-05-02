// Controllers/OrderController.cs
using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OrderController(AppDbContext context) => _context = context;

        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .ToListAsync();
            return Ok(orders);
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound("Order not found");
            return Ok(order);
        }

        // GET: api/orders/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(long userId)
        {
            var orders = await _context.Orders
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .Include(o => o.Payment)
                .Where(o => o.UserId == userId)
                .ToListAsync();
            return Ok(orders);
        }

        // POST: api/orders
        // Body: { userId, productIds: [1,2,3] }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest req)
        {
            var user = await _context.Users.FindAsync(req.UserId);
            if (user == null) return NotFound("User not found");

            var products = await _context.Products
                .Where(p => req.ProductIds.Contains(p.Id))
                .ToListAsync();

            if (!products.Any()) return BadRequest("Không có sản phẩm hợp lệ");

            // Kiểm tra đã mua chưa
            var alreadyOwned = await _context.Libraries
                .Where(l => l.UserId == req.UserId && req.ProductIds.Contains(l.ProductId))
                .Select(l => l.ProductId)
                .ToListAsync();

            if (alreadyOwned.Any())
            {
                var names = products.Where(p => alreadyOwned.Contains(p.Id)).Select(p => p.Name);
                return BadRequest($"Bạn đã sở hữu: {string.Join(", ", names)}");
            }

            var total = products.Sum(p => p.Price);

            var order = new Order
            {
                UserId = req.UserId,
                TotalPrice = total,
                PaymentStatus = "pending",
                OrderStatus = "processing",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Items = products.Select(p => new OrderItem
                {
                    ProductId = p.Id,
                    Price = p.Price,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            _context.OrderItems.RemoveRange(order.Items!);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }
    }

    public class CreateOrderRequest
    {
        public long UserId { get; set; }
        public List<long> ProductIds { get; set; } = new();
    }
}