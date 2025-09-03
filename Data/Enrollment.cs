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

    // Novos campos (armazenar apenas dígitos para MobilePhone e Cpf)
    [Required, MaxLength(11)]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Informe 11 dígitos (DDD + número)")]
    public string MobilePhone { get; set; } = default!;

    [Required, MaxLength(11)]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Informe o CPF com 11 dígitos")]
    public string Cpf { get; set; } = default!;

    [Required, MaxLength(20)]
    [RegularExpression("^(Masculino|Feminino|Outro)$", ErrorMessage = "Valores válidos: Masculino, Feminino ou Outro")]
    public string Gender { get; set; } = default!;

    [Required, MaxLength(200)]
    public string Organization { get; set; } = default!;

    [Required]
    public int DegreeId { get; set; }
    public Degree Degree { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
