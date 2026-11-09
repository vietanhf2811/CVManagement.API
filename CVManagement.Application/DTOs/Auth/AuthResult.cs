namespace CVManagement.Application.DTOs.Auth;

public class AuthResult
{
    public LoginResponseDto Response { get; set; } = default!;

    public string RefreshToken { get; set; } = string.Empty;
}
