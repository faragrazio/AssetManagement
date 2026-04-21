using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Interfaces;
using MediatR;

namespace AssetManagement.Application.MaintenanceOrders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler
    : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    private readonly IMaintenanceOrderRepository _orderRepository;
    private readonly IAssetRepository _assetRepository;

    public UpdateOrderStatusCommandHandler(
        IMaintenanceOrderRepository orderRepository,
        IAssetRepository assetRepository)
    {
        _orderRepository = orderRepository;
        _assetRepository = assetRepository;
    }

    public async Task<Result> Handle(
        UpdateOrderStatusCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Cerca l'ordine
        var order = await _orderRepository.GetByIdAsync(
            request.OrderId, cancellationToken);

        if (order == null)
            return Result.Failure($"Ordine con ID '{request.OrderId}' non trovato.");

        // 2. Applica la transizione di stato tramite i metodi dell'entità
        switch (request.NewStatus)
        {
            case OrderStatus.InProgress:
                order.Start();
                break;

            case OrderStatus.Completed:
                order.Complete(request.CompletionNotes);

                // Quando l'ordine è completato, riporta l'asset ad Active
                var asset = await _assetRepository.GetByIdAsync(
                    order.AssetId, cancellationToken);

                if (asset != null)
                {
                    asset.CompleteMaintenance();
                    _assetRepository.Update(asset);
                }
                break;

            case OrderStatus.Cancelled:
                order.Cancel();
                break;

            default:
                return Result.Failure($"Transizione di stato non supportata: '{request.NewStatus}'.");
        }

        // 3. Salva
        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}