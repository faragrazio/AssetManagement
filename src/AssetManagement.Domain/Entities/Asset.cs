using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Exceptions;

namespace AssetManagement.Domain.Entities;

// Rappresenta un asset aziendale (macchinario, attrezzatura, veicolo, ecc.)
public class Asset
{
    // Chiave primaria — setter privato: solo il costruttore o EF Core possono impostarlo
    public int Id { get; private set; }

    // Nome descrittivo dell'asset (es. "Tornio CNC Reparto 3")
    public string Name { get; private set; } = string.Empty;

    // Numero seriale univoco del produttore
    public string SerialNumber { get; private set; } = string.Empty;

    // Categoria dell'asset (es. "Macchinario", "Veicolo", "Strumento")
    public string Category { get; private set; } = string.Empty;

    // Posizione fisica dell'asset all'interno dell'azienda
    public string Location { get; private set; } = string.Empty;

    // Stato operativo corrente
    public AssetStatus Status { get; private set; }

    // Data di acquisto o messa in servizio
    public DateTime PurchaseDate { get; private set; }

    // Data e ora di creazione del record (impostata automaticamente)
    public DateTime CreatedAt { get; private set; }

    // Data e ora dell'ultimo aggiornamento
    public DateTime UpdatedAt { get; private set; }

    // Costruttore privato richiesto da EF Core per la materializzazione
    // EF Core crea oggetti tramite reflection — senza questo costruttore fallisce
    private Asset() { }

    // Costruttore pubblico — unico modo valido per creare un nuovo Asset
    public Asset(string name, string serialNumber, string category, string location, DateTime purchaseDate)
    {
        // Validazione immediata: non ammettiamo asset con dati mancanti
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Il nome dell'asset non può essere vuoto.");

        if (string.IsNullOrWhiteSpace(serialNumber))
            throw new DomainException("Il numero seriale non può essere vuoto.");

        if (string.IsNullOrWhiteSpace(category))
            throw new DomainException("La categoria non può essere vuota.");

        if (string.IsNullOrWhiteSpace(location))
            throw new DomainException("La posizione non può essere vuota.");

        if (purchaseDate > DateTime.UtcNow)
            throw new DomainException("La data di acquisto non può essere nel futuro.");

        Name = name;
        SerialNumber = serialNumber;
        Category = category;
        Location = location;
        PurchaseDate = purchaseDate;
        Status = AssetStatus.Active; // Un nuovo asset è sempre Active
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Aggiorna i dati descrittivi dell'asset
    public void Update(string name, string category, string location)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Il nome dell'asset non può essere vuoto.");

        Name = name;
        Category = category;
        Location = location;
        UpdatedAt = DateTime.UtcNow;
    }

    // Mette l'asset in manutenzione — solo se è attivo
    public void StartMaintenance()
    {
        if (Status != AssetStatus.Active)
            throw new DomainException($"Impossibile avviare manutenzione: lo stato corrente è '{Status}'.");

        Status = AssetStatus.InMaintenance;
        UpdatedAt = DateTime.UtcNow;
    }

    // Riporta l'asset attivo dopo la manutenzione
    public void CompleteMaintenance()
    {
        if (Status != AssetStatus.InMaintenance)
            throw new DomainException("Impossibile completare manutenzione: l'asset non è in manutenzione.");

        Status = AssetStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    // Dismette definitivamente l'asset — operazione irreversibile
    public void Decommission()
    {
        if (Status == AssetStatus.Decommissioned)
            throw new DomainException("L'asset è già dismesso.");

        Status = AssetStatus.Decommissioned;
        UpdatedAt = DateTime.UtcNow;
    }
}