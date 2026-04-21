namespace AssetManagement.Application.Common.Interfaces;

// Contratto minimo del DbContext esposto all'Application layer
// Gli Handler usano i Repository — questo contratto serve solo per SaveChangesAsync
public interface IApplicationDbContext
{
    // Persiste tutte le modifiche pendenti nel DB
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}