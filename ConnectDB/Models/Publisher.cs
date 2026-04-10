using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models;

public class Publisher
{
    [Key]
    public long Id { get; set; }

    [Required, StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Website { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}