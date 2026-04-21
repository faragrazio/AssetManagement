namespace AssetManagement.Domain.Exceptions;

// Eccezione base per tutte le violazioni di regole di business del Domain
public class DomainException : Exception
{
    // Costruttore con solo messaggio
    public DomainException(string message) : base(message)
    {
    }

    // Costruttore con messaggio e causa originale (inner exception)
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}