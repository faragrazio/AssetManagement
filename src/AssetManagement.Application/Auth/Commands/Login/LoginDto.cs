namespace AssetManagement.Application.Auth.Commands.Login;

// Risposta al login — contiene il token JWT e i dati base dell'utente
public class LoginDto
{
    public string Token { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}