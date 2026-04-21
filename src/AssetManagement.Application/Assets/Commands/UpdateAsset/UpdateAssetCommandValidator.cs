using FluentValidation;

namespace AssetManagement.Application.Assets.Commands.UpdateAsset;

public class UpdateAssetCommandValidator : AbstractValidator<UpdateAssetCommand>
{
    public UpdateAssetCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID asset non valido.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Il nome dell'asset è obbligatorio.")
            .MaximumLength(200).WithMessage("Il nome non può superare 200 caratteri.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("La categoria è obbligatoria.")
            .MaximumLength(100).WithMessage("La categoria non può superare 100 caratteri.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("La posizione è obbligatoria.")
            .MaximumLength(200).WithMessage("La posizione non può superare 200 caratteri.");
    }
}