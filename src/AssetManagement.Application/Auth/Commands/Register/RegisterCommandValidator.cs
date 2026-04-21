using AssetManagement.Application.Auth.Commands.Register;
using FluentValidation;

namespace AssetManagement.Application.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    // Ruoli validi nel sistema
    private static readonly string[] ValidRoles = { "Admin", "Technician", "Viewer" };

    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Il nome è obbligatorio.")
            .MaximumLength(100).WithMessage("Il nome non può superare 100 caratteri.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Il cognome è obbligatorio.")
            .MaximumLength(100).WithMessage("Il cognome non può superare 100 caratteri.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("L'email è obbligatoria.")
            .EmailAddress().WithMessage("Formato email non valido.")
            .MaximumLength(200).WithMessage("L'email non può superare 200 caratteri.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La password è obbligatoria.")
            .MinimumLength(8).WithMessage("La password deve avere almeno 8 caratteri.")
            .Matches("[A-Z]").WithMessage("La password deve contenere almeno una lettera maiuscola.")
            .Matches("[0-9]").WithMessage("La password deve contenere almeno un numero.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Il ruolo è obbligatorio.")
            .Must(r => ValidRoles.Contains(r))
            .WithMessage($"Ruolo non valido. Ruoli accettati: {string.Join(", ", ValidRoles)}.");
    }
}