using AssetManagement.Application.Common.Interfaces;
using AssetManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Persistence;

// Classe centrale di EF Core — rappresenta la connessione al database PostgreSQL
// Implementa IApplicationDbContext per rispettare il contratto definito in Application
public class AssetManagementDbContext : DbContext, IApplicationDbContext
{
    public AssetManagementDbContext(DbContextOptions<AssetManagementDbContext> options)
        : base(options)
    {
    }

    // Ogni DbSet rappresenta una tabella nel database
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<MaintenanceOrder> MaintenanceOrders => Set<MaintenanceOrder>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Applica automaticamente tutte le configurazioni IEntityTypeConfiguration
        // presenti nell'assembly corrente (cartella Configurations/)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssetManagementDbContext).Assembly);
    }
}