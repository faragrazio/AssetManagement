using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.MaintenanceOrders.Queries;

public class MaintenanceOrderDto
{
    public int Id { get; init; }
    public int AssetId { get; init; }
    public string AssetName { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Priority Priority { get; init; }
    public string PriorityName { get; init; } = string.Empty;
    public OrderStatus Status { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public string AssignedTo { get; init; } = string.Empty;
    public DateTime ScheduledDate { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? CompletionNotes { get; init; }
    public DateTime CreatedAt { get; init; }

    public MaintenanceOrderDto(MaintenanceOrder order)
    {
        Id = order.Id;
        AssetId = order.AssetId;
        // Se la navigation property Asset è caricata, prendi il nome — altrimenti stringa vuota
        AssetName = order.Asset?.Name ?? string.Empty;
        Title = order.Title;
        Description = order.Description;
        Priority = order.Priority;
        PriorityName = order.Priority.ToString();
        Status = order.Status;
        StatusName = order.Status.ToString();
        AssignedTo = order.AssignedTo;
        ScheduledDate = order.ScheduledDate;
        CompletedAt = order.CompletedAt;
        CompletionNotes = order.CompletionNotes;
        CreatedAt = order.CreatedAt;
    }
}