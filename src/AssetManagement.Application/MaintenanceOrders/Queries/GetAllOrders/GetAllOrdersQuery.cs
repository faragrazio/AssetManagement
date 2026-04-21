using AssetManagement.Application.Common.Models;
using MediatR;

namespace AssetManagement.Application.MaintenanceOrders.Queries.GetAllOrders;

// Recupera tutti gli ordini — filtro opzionale per stato
public record GetAllOrdersQuery(string? Status = null): IRequest<Result<IEnumerable<MaintenanceOrderDto>>>;