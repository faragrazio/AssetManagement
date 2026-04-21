using FluentValidation;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.MaintenanceOrders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandValidator
    : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0).WithMessage("ID ordine non valido.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Stato non valido.")
            // Non si può impostare direttamente Pending — è lo stato iniziale
            .NotEqual(OrderStatus.Pending)
            .WithMessage("Non è possibile impostare lo stato Pending manualmente.");

        // Le note di completamento sono obbligatorie solo se lo stato è Completed
        RuleFor(x => x.CompletionNotes)
            .MaximumLength(1000)
            .WithMessage("Le note non possono superare 1000 caratteri.")
            .When(x => x.CompletionNotes != null);
    }
}