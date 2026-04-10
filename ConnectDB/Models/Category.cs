using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConnectDB.Models;

public class Category
{
    [Key]
    public long Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public ICollection<ProductCategory>? ProductCategories { get; set; }

}