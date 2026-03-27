using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models;

public class Order
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long UserId { get; set; }

    public ApplicationUser? User { get; set; }

    [Required]
    public decimal TotalPrice { get; set; }

    [Required]
    public string PaymentStatus { get; set; } = "pending";

    [Required]
    public string OrderStatus { get; set; } = "processing";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<OrderItem>? Items { get; set; }
    public Payment? Payment { get; set; }
}