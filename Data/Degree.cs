using System.ComponentModel.DataAnnotations;

namespace WebApp.Data;

public class Degree
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = default!;
}
