using AssetManagement.Application.Common.Models;
using MediatR;

namespace AssetManagement.Application.MaintenanceOrders.Queries.GetOrdersByAsset;

// Recupera tutti gli ordini di manutenzione di un asset specifico
public record GetOrdersByAssetQuery(int AssetId): IRequest<Result<IEnumerable<MaintenanceOrderDto>>>;