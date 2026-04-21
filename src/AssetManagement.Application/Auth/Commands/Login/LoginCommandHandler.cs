using AssetManagement.Application.Common.Interfaces;
using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Interfaces;
using MediatR;

namespace AssetManagement.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Cerca l'utente per email
        var user = await _userRepository.GetByEmailAsync(
            request.Email.ToLowerInvariant(), cancellationToken);

        // Messaggio generico intenzionale — non rivelare se l'email esiste o no
        if (user == null)
            return Result<LoginDto>.Failure("Credenziali non valide.");

        // 2. Verifica che l'utente sia attivo
        if (!user.IsActive)
            return Result<LoginDto>.Failure("Account disattivato.");

        // 3. Verifica la password tramite BCrypt
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<LoginDto>.Failure("Credenziali non valide.");

        // 4. Genera il token JWT
        var token = _jwtService.GenerateToken(user);

        var dto = new LoginDto
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddHours(24) // scadenza token
        };

        return Result<LoginDto>.Success(dto);
    }
}