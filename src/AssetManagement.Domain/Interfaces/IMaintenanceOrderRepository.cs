using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Domain.Interfaces;

// Interfaccia specifica per MaintenanceOrder
public interface IMaintenanceOrderRepository : IRepository<MaintenanceOrder>
{
    // Recupera tutti gli ordini di un asset specifico
    Task<IEnumerable<MaintenanceOrder>> GetByAssetIdAsync(int assetId, CancellationToken cancellationToken = default);

    // Recupera tutti gli ordini filtrati per stato
    Task<IEnumerable<MaintenanceOrder>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);

    // Recupera gli ordini assegnati a un tecnico specifico
    Task<IEnumerable<MaintenanceOrder>> GetByAssignedToAsync(string assignedTo, CancellationToken cancellationToken = default);

    // Recupera gli ordini in scadenza entro una certa data
    Task<IEnumerable<MaintenanceOrder>> GetOverdueAsync(DateTime referenceDate, CancellationToken cancellationToken = default);
}