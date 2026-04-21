using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Enums;
using MediatR;

namespace AssetManagement.Application.MaintenanceOrders.Commands.CreateOrder;

// Crea un nuovo ordine di manutenzione su un asset
public record CreateOrderCommand(int AssetId, string Title, string Description, Priority Priority, string AssignedTo, DateTime ScheduledDate) : IRequest<Result<int>>;