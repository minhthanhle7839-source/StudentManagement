// Controllers/OrderItemController.cs
using ConnectDB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/orderitems")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OrderItemController(AppDbContext context) => _context = context;

        // GET: api/orderitems/order/{orderId}
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrder(long orderId)
        {
            var items = await _context.OrderItems
                .Include(i => i.Product).ThenInclude(p => p!.Images)
                .Where(i => i.OrderId == orderId)
                .ToListAsync();
            return Ok(items);
        }

        // GET: api/orderitems/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var item = await _context.OrderItems
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }
    }
}