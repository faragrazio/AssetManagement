using FluentValidation;

namespace AssetManagement.Application.Assets.Commands.CreateAsset;

// Regole di validazione per CreateAssetCommand
// Vengono eseguite automaticamente dal ValidationBehavior
public class CreateAssetCommandValidator : AbstractValidator<CreateAssetCommand>
{
    public CreateAssetCommandValidator()
    {
        // Name: obbligatorio, max 200 caratteri
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Il nome dell'asset è obbligatorio.")
            .MaximumLength(200).WithMessage("Il nome non può superare 200 caratteri.");

        // SerialNumber: obbligatorio, max 100 caratteri
        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("Il numero seriale è obbligatorio.")
            .MaximumLength(100).WithMessage("Il numero seriale non può superare 100 caratteri.");

        // Category: obbligatoria, max 100 caratteri
        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("La categoria è obbligatoria.")
            .MaximumLength(100).WithMessage("La categoria non può superare 100 caratteri.");

        // Location: obbligatoria, max 200 caratteri
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("La posizione è obbligatoria.")
            .MaximumLength(200).WithMessage("La posizione non può superare 200 caratteri.");

        // PurchaseDate: non può essere nel futuro
        RuleFor(x => x.PurchaseDate)
            .NotEmpty().WithMessage("La data di acquisto è obbligatoria.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("La data di acquisto non può essere nel futuro.");
    }
}