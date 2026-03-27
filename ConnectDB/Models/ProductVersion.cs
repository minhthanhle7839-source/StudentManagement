using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models;

public class ProductVersion
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long ProductId { get; set; }

    public Product? Product { get; set; }

    [Required, StringLength(50)]
    public string Version { get; set; } = string.Empty;

    public string? Changelog { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public long? FileSize { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}