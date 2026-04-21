using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Enums;
using MediatR;

namespace AssetManagement.Application.MaintenanceOrders.Commands.UpdateOrderStatus;

// Aggiorna lo stato di un ordine di manutenzione
public record UpdateOrderStatusCommand(
    int OrderId,
    OrderStatus NewStatus,
    string? CompletionNotes = null  // opzionale — usato solo quando NewStatus = Completed
) : IRequest<Result>;