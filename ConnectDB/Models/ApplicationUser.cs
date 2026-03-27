using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models;

public class ApplicationUser
{
    [Key]
    public long Id { get; set; }

    [Required, StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(255)]
    public string? Avatar { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    public int Status { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}