using AssetManagement.Application.Common.Models;
using MediatR;

namespace AssetManagement.Application.Auth.Commands.Register;

// Registra un nuovo utente nel sistema
public record RegisterCommand(string FirstName, string LastName, string Email, string Password,string Role) : IRequest<Result<int>>;