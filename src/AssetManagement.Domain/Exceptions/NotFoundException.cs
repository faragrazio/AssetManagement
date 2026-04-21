namespace AssetManagement.Domain.Exceptions;

// Lanciata quando una risorsa richiesta non esiste nel sistema
public class NotFoundException : DomainException
{
    // Costruttore generico con messaggio libero
    public NotFoundException(string message) : base(message)
    {
    }

    // Costruttore tipizzato: accetta il nome dell'entità e il suo ID
    // Esempio: new NotFoundException(nameof(Asset), 42)
    // Produce: "Asset con ID '42' non trovato."
    public NotFoundException(string entityName, object key)
        : base($"{entityName} con ID '{key}' non trovato.")
    {
    }
}