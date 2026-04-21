using AssetManagement.Application.Common.Models;
using MediatR;

namespace AssetManagement.Application.Auth.Commands.Login;

// Richiesta di login con email e password
public record LoginCommand(string Email, string Password) : IRequest<Result<LoginDto>>;