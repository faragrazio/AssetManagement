using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.Assets.Queries;

// Oggetto che rappresenta un Asset come viene restituito al client via HTTP
public class AssetDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public AssetStatus Status { get; init; }
    public string StatusName { get; init; } = string.Empty;
    public DateTime PurchaseDate { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    // Costruttore che mappa l'entità Domain → DTO
    public AssetDto(Asset asset)
    {
        Id = asset.Id;
        Name = asset.Name;
        SerialNumber = asset.SerialNumber;
        Category = asset.Category;
        Location = asset.Location;
        Status = asset.Status;
        // Converte l'enum in stringa leggibile (es. "InMaintenance" → "InMaintenance")
        StatusName = asset.Status.ToString();
        PurchaseDate = asset.PurchaseDate;
        CreatedAt = asset.CreatedAt;
        UpdatedAt = asset.UpdatedAt;
    }
}