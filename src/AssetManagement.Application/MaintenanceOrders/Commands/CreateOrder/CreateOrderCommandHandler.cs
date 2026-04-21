using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using MediatR;

namespace AssetManagement.Application.MaintenanceOrders.Commands.CreateOrder;

public class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, Result<int>>
{
    private readonly IMaintenanceOrderRepository _orderRepository;
    private readonly IAssetRepository _assetRepository;

    public CreateOrderCommandHandler(
        IMaintenanceOrderRepository orderRepository,
        IAssetRepository assetRepository)
    {
        _orderRepository = orderRepository;
        _assetRepository = assetRepository;
    }

    public async Task<Result<int>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Verifica che l'asset esista
        var asset = await _assetRepository.GetByIdAsync(
            request.AssetId, cancellationToken);

        if (asset == null)
            return Result<int>.Failure(
                $"Asset con ID '{request.AssetId}' non trovato.");

        // 2. Metti l'asset in manutenzione — applica regole di business
        // Se l'asset non è Active lancia DomainException
        asset.StartMaintenance();
        _assetRepository.Update(asset);

        // 3. Crea il nuovo ordine
        var order = new MaintenanceOrder(
            request.AssetId,
            request.Title,
            request.Description,
            request.Priority,
            request.AssignedTo,
            request.ScheduledDate
        );

        // 4. Salva ordine e aggiornamento asset
        await _orderRepository.AddAsync(order, cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(order.Id);
    }
}