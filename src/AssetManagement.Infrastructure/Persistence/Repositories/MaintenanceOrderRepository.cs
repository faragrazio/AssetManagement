using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Persistence.Repositories;

public class MaintenanceOrderRepository : IMaintenanceOrderRepository
{
    private readonly AssetManagementDbContext _context;

    public MaintenanceOrderRepository(AssetManagementDbContext context)
    {
        _context = context;
    }

    public async Task<MaintenanceOrder?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        return await _context.MaintenanceOrders
            .Include(o => o.Asset) // carica la navigation property Asset
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.MaintenanceOrders
            .Include(o => o.Asset)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MaintenanceOrder entity,
        CancellationToken cancellationToken = default)
    {
        await _context.MaintenanceOrders.AddAsync(entity, cancellationToken);
    }

    public void Update(MaintenanceOrder entity)
    {
        _context.MaintenanceOrders.Update(entity);
    }

    public void Remove(MaintenanceOrder entity)
    {
        _context.MaintenanceOrders.Remove(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetByAssetIdAsync(
        int assetId, CancellationToken cancellationToken = default)
    {
        return await _context.MaintenanceOrders
            .Include(o => o.Asset)
            .Where(o => o.AssetId == assetId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetByStatusAsync(
        OrderStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.MaintenanceOrders
            .Include(o => o.Asset)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetByAssignedToAsync(
        string assignedTo, CancellationToken cancellationToken = default)
    {
        return await _context.MaintenanceOrders
            .Include(o => o.Asset)
            .Where(o => o.AssignedTo == assignedTo)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MaintenanceOrder>> GetOverdueAsync(
        DateTime referenceDate, CancellationToken cancellationToken = default)
    {
        // Ordini in scadenza: Pending o InProgress con data pianificata passata
        return await _context.MaintenanceOrders
            .Include(o => o.Asset)
            .Where(o => o.ScheduledDate < referenceDate &&
                       (o.Status == OrderStatus.Pending || o.Status == OrderStatus.InProgress))
            .OrderBy(o => o.ScheduledDate)
            .ToListAsync(cancellationToken);
    }
}