using FluentValidation;

namespace AssetManagement.Application.MaintenanceOrders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.AssetId)
            .GreaterThan(0).WithMessage("ID asset non valido.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Il titolo è obbligatorio.")
            .MaximumLength(200).WithMessage("Il titolo non può superare 200 caratteri.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descrizione è obbligatoria.")
            .MaximumLength(1000).WithMessage("La descrizione non può superare 1000 caratteri.");

        RuleFor(x => x.AssignedTo)
            .NotEmpty().WithMessage("Il tecnico assegnato è obbligatorio.")
            .MaximumLength(200).WithMessage("Il nome tecnico non può superare 200 caratteri.");

        RuleFor(x => x.ScheduledDate)
            .NotEmpty().WithMessage("La data pianificata è obbligatoria.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("La data pianificata non può essere nel passato.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priorità non valida.");
    }
}