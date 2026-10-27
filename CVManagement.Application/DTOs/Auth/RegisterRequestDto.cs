
using System.ComponentModel.DataAnnotations;

namespace CVManagement.Application.DTOs.Auth;

public class RegisterRequestDto
{
    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Phone { get; set; }
}
