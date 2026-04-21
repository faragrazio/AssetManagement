using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Interfaces;
using MediatR;

namespace AssetManagement.Application.MaintenanceOrders.Queries.GetOrdersByAsset;

public class GetOrdersByAssetQueryHandler
    : IRequestHandler<GetOrdersByAssetQuery, Result<IEnumerable<MaintenanceOrderDto>>>
{
    private readonly IMaintenanceOrderRepository _orderRepository;

    public GetOrdersByAssetQueryHandler(IMaintenanceOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<IEnumerable<MaintenanceOrderDto>>> Handle(
        GetOrdersByAssetQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByAssetIdAsync(
            request.AssetId, cancellationToken);

        var dtos = orders.Select(o => new MaintenanceOrderDto(o));

        return Result<IEnumerable<MaintenanceOrderDto>>.Success(dtos);
    }
}