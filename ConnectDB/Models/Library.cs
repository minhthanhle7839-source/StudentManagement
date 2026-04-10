using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models;

public class Library
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long UserId { get; set; }

    public ApplicationUser? User { get; set; }

    [Required]
    public long ProductId { get; set; }

    public Product? Product { get; set; }

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    public string? LicenseKey { get; set; }

    public int Status { get; set; } = 1;
}