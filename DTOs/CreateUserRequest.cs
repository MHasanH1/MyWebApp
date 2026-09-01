using System.ComponentModel.DataAnnotations;

namespace MyWebApp.DTOs;

public class CreateUserRequest
{
    [Required]
    [MinLength(2)]
    [MaxLength(25)]
    public string Name { get; set; } = "";
    [Range(8, 120)]
    public int Age { get; set; }
}