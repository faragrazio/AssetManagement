using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Exceptions;

namespace AssetManagement.Domain.Entities;

// Rappresenta un ordine di manutenzione su un asset aziendale
public class MaintenanceOrder
{
    public int Id { get; private set; }

    // ID dell'asset su cui viene eseguita la manutenzione (chiave esterna)
    public int AssetId { get; private set; }

    // Navigazione EF Core verso l'asset — null finché non viene caricato dal DB
    public Asset? Asset { get; private set; }

    // Titolo breve dell'intervento (es. "Sostituzione cinghia motore")
    public string Title { get; private set; } = string.Empty;

    // Descrizione dettagliata del problema o dell'intervento pianificato
    public string Description { get; private set; } = string.Empty;

    // Priorità dell'intervento
    public Priority Priority { get; private set; }

    // Stato corrente dell'ordine
    public OrderStatus Status { get; private set; }

    // Tecnico assegnato all'intervento
    public string AssignedTo { get; private set; } = string.Empty;

    // Data pianificata per l'intervento
    public DateTime ScheduledDate { get; private set; }

    // Data effettiva di completamento — null finché non viene completato
    public DateTime? CompletedAt { get; private set; }

    // Note del tecnico al completamento
    public string? CompletionNotes { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Costruttore privato per EF Core
    private MaintenanceOrder() { }

    // Costruttore pubblico — crea un nuovo ordine in stato Pending
    public MaintenanceOrder(int assetId, string title, string description,
        Priority priority, string assignedTo, DateTime scheduledDate)
    {
        if (assetId <= 0)
            throw new DomainException("ID asset non valido.");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Il titolo dell'ordine non può essere vuoto.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("La descrizione non può essere vuota.");

        if (string.IsNullOrWhiteSpace(assignedTo))
            throw new DomainException("Il tecnico assegnato non può essere vuoto.");

        if (scheduledDate < DateTime.UtcNow.Date)
            throw new DomainException("La data pianificata non può essere nel passato.");

        AssetId = assetId;
        Title = title;
        Description = description;
        Priority = priority;
        AssignedTo = assignedTo;
        ScheduledDate = scheduledDate;
        Status = OrderStatus.Pending; // Ogni nuovo ordine parte da Pending
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Avvia la lavorazione dell'ordine
    public void Start()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Impossibile avviare: lo stato corrente è '{Status}'.");

        Status = OrderStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    // Completa l'ordine con note opzionali del tecnico
    public void Complete(string? notes = null)
    {
        if (Status != OrderStatus.InProgress)
            throw new DomainException($"Impossibile completare: lo stato corrente è '{Status}'.");

        Status = OrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CompletionNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    // Annulla l'ordine — possibile solo da Pending o InProgress
    public void Cancel()
    {
        if (Status == OrderStatus.Completed)
            throw new DomainException("Impossibile annullare un ordine già completato.");

        if (Status == OrderStatus.Cancelled)
            throw new DomainException("L'ordine è già annullato.");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}