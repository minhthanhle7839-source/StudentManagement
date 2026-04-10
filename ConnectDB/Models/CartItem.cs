using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models;

public class CartItem
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long CartId { get; set; }

    public Cart? Cart { get; set; }

    [Required]
    public long ProductId { get; set; }

    public Product? Product { get; set; }

    [Required]
    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}