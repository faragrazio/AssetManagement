using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Interfaces;
using MediatR;

namespace AssetManagement.Application.MaintenanceOrders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler
    : IRequestHandler<GetAllOrdersQuery, Result<IEnumerable<MaintenanceOrderDto>>>
{
    private readonly IMaintenanceOrderRepository _orderRepository;

    public GetAllOrdersQueryHandler(IMaintenanceOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<IEnumerable<MaintenanceOrderDto>>> Handle(
        GetAllOrdersQuery request,
        CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.MaintenanceOrder> orders;

        // Se è specificato uno stato, filtra — altrimenti prendi tutto
        if (request.Status != null &&
            Enum.TryParse<OrderStatus>(request.Status, true, out var status))
        {
            orders = await _orderRepository.GetByStatusAsync(status, cancellationToken);
        }
        else
        {
            orders = await _orderRepository.GetAllAsync(cancellationToken);
        }

        var dtos = orders.Select(o => new MaintenanceOrderDto(o));
        return Result<IEnumerable<MaintenanceOrderDto>>.Success(dtos);
    }
}