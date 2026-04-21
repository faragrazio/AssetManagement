using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Domain.Interfaces;

// Interfaccia specifica per Asset — estende le operazioni base con query di dominio
public interface IAssetRepository : IRepository<Asset>
{
    // Cerca un asset per numero seriale — utile per evitare duplicati
    Task<Asset?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);

    // Recupera tutti gli asset filtrati per stato
    Task<IEnumerable<Asset>> GetByStatusAsync(AssetStatus status, CancellationToken cancellationToken = default);

    // Recupera tutti gli asset filtrati per categoria
    Task<IEnumerable<Asset>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);

    // Verifica se esiste già un asset con quel numero seriale (escludendo l'ID corrente)
    // Usato in fase di aggiornamento per evitare conflitti
    Task<bool> SerialNumberExistsAsync(string serialNumber, int excludeId = 0, CancellationToken cancellationToken = default);
}