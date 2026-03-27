using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models;

public class Review
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long UserId { get; set; }

    public ApplicationUser? User { get; set; }

    [Required]
    public long ProductId { get; set; }

    public Product? Product { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public int Status { get; set; } = 1;
}