using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models;

public class Cart
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long UserId { get; set; }

    public ApplicationUser? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public ICollection<CartItem>? Items { get; set; }
}