// Controllers/PaymentController.cs
using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PaymentController(AppDbContext context) => _context = context;

        // GET: api/payments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _context.Payments
                .Include(p => p.Order).ThenInclude(o => o!.Items)
                .ToListAsync();
            return Ok(payments);
        }

        // GET: api/payments/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var payment = await _context.Payments
                .Include(p => p.Order).ThenInclude(o => o!.Items)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        // POST: api/payments
        // Thanh toán → cập nhật Order → thêm vào Library
        [HttpPost]
        public async Task<IActionResult> Pay([FromBody] PayRequest req)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == req.OrderId);

            if (order == null) return NotFound("Order not found");
            if (order.PaymentStatus == "paid") return BadRequest("Order đã được thanh toán");

            // 1. Tạo payment
            var payment = new Payment
            {
                OrderId = order.Id,
                PaymentMethod = req.PaymentMethod,
                TransactionId = req.TransactionId ?? Guid.NewGuid().ToString(),
                Amount = order.TotalPrice,
                Status = "success",
                CreatedAt = DateTime.UtcNow
            };
            _context.Payments.Add(payment);

            // 2. Cập nhật order
            order.PaymentStatus = "paid";
            order.OrderStatus = "completed";
            order.UpdatedAt = DateTime.UtcNow;

            // 3. Thêm vào Library
            foreach (var item in order.Items!)
            {
                var alreadyInLib = await _context.Libraries
                    .AnyAsync(l => l.UserId == order.UserId && l.ProductId == item.ProductId);

                if (!alreadyInLib)
                {
                    _context.Libraries.Add(new Library
                    {
                        UserId = order.UserId,
                        ProductId = item.ProductId,
                        PurchaseDate = DateTime.UtcNow,
                        LicenseKey = GenerateLicenseKey(),
                        Status = 1
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { payment, order });
        }

        private static string GenerateLicenseKey()
        {
            return string.Join("-", Enumerable.Range(0, 4)
                .Select(_ => Guid.NewGuid().ToString("N")[..6].ToUpper()));
        }
    }

    public class PayRequest
    {
        public long OrderId { get; set; }
        public string PaymentMethod { get; set; } = "card";
        public string? TransactionId { get; set; }
    }
}