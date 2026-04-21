using AssetManagement.Application.Common.Interfaces;
using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using MediatR;

namespace AssetManagement.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<int>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<int>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Verifica che l'email non sia già registrata
        var emailExists = await _userRepository.EmailExistsAsync(
            request.Email.ToLowerInvariant(), cancellationToken);

        if (emailExists)
            return Result<int>.Failure("Email già registrata.");

        // 2. Hash della password con BCrypt
        var passwordHash = _passwordHasher.Hash(request.Password);

        // 3. Crea il nuovo utente
        var user = new User(
            request.FirstName,
            request.LastName,
            request.Email,
            passwordHash,
            request.Role
        );

        // 4. Salva e restituisce l'ID
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(user.Id);
    }
}