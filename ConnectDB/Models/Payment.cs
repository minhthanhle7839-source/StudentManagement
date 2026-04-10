using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models;

public class Payment
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long OrderId { get; set; }

    public Order? Order { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    public string? TransactionId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}