using FluentValidation;

namespace AssetManagement.Application.Assets.Commands.DeleteAsset;

public class DeleteAssetCommandValidator : AbstractValidator<DeleteAssetCommand>
{
    public DeleteAssetCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID asset non valido.");
    }
}