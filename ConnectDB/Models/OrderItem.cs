using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models;

public class OrderItem
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long OrderId { get; set; }

    public Order? Order { get; set; }

    [Required]
    public long ProductId { get; set; }

    public Product? Product { get; set; }

    [Required]
    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}