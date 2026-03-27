using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models;

public class Wishlist
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long UserId { get; set; }

    public ApplicationUser? User { get; set; }

    [Required]
    public long ProductId { get; set; }

    public Product? Product { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}