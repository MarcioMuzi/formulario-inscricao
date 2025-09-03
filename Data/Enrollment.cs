using System.ComponentModel.DataAnnotations;

namespace WebApp.Data;

public class Enrollment
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string FullName { get; set; } = default!;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = default!;

    [MaxLength(100)]
    public string? Course { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
