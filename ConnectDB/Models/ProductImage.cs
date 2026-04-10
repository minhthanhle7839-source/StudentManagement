using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConnectDB.Models;

public class ProductImage
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long ProductId { get; set; }
    [JsonIgnore]
    public Product? Product { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    public int? DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}