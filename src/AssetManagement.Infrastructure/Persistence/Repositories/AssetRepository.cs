using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Interfaces;
using AssetManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Persistence.Repositories;

// Implementazione concreta di IAssetRepository usando EF Core + Dapper
public class AssetRepository : IAssetRepository
{
    private readonly AssetManagementDbContext _context;

    public AssetRepository(AssetManagementDbContext context)
    {
        _context = context;
    }

    // ── Operazioni base (IRepository<Asset>) ──────────────────────────

    public async Task<Asset?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Asset>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Asset entity, CancellationToken cancellationToken = default)
    {
        await _context.Assets.AddAsync(entity, cancellationToken);
    }

    public void Update(Asset entity)
    {
        // EF Core traccia automaticamente le modifiche — Update marca l'entità come Modified
        _context.Assets.Update(entity);
    }

    public void Remove(Asset entity)
    {
        _context.Assets.Remove(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // ── Operazioni specifiche (IAssetRepository) ──────────────────────

    public async Task<Asset?> GetBySerialNumberAsync(
        string serialNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .FirstOrDefaultAsync(a => a.SerialNumber == serialNumber, cancellationToken);
    }

    public async Task<IEnumerable<Asset>> GetByStatusAsync(
        AssetStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Where(a => a.Status == status)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Asset>> GetByCategoryAsync(
        string category, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .Where(a => a.Category.ToLower() == category.ToLower())
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> SerialNumberExistsAsync(
        string serialNumber, int excludeId = 0, CancellationToken cancellationToken = default)
    {
        return await _context.Assets
            .AnyAsync(a => a.SerialNumber == serialNumber && a.Id != excludeId,
                cancellationToken);
    }
}