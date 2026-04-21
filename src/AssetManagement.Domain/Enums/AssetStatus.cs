namespace AssetManagement.Domain.Enums;

// Rappresenta lo stato operativo di un asset aziendale
public enum AssetStatus
{
    Active = 1,       // Asset operativo e disponibile
    InMaintenance = 2, // Asset temporaneamente fuori servizio per manutenzione
    Decommissioned = 3 // Asset dismesso definitivamente
}