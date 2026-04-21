namespace AssetManagement.Application.Common.Models;

// Wrapper generico per il risultato di un'operazione
// Evita di usare eccezioni per flussi di business normali (es. "utente non trovato")
public class Result<T>
{
    // Valore restituito in caso di successo — null se fallimento
    public T? Value { get; private set; }

    // Messaggio di errore in caso di fallimento — null se successo
    public string? Error { get; private set; }

    // Indica se l'operazione è andata a buon fine
    public bool IsSuccess { get; private set; }

    // Proprietà inversa per comodità
    public bool IsFailure => !IsSuccess;

    // Costruttore privato — si creano istanze solo tramite i metodi statici
    private Result(T? value, bool isSuccess, string? error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    // Factory method per il successo — es. Result<AssetDto>.Success(dto)
    public static Result<T> Success(T value) =>
        new(value, true, null);

    // Factory method per il fallimento — es. Result<AssetDto>.Failure("Asset non trovato")
    public static Result<T> Failure(string error) =>
        new(default, false, error);
}

// Versione non generica — per operazioni che non restituiscono un valore (es. Delete)
public class Result
{
    public string? Error { get; private set; }
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;

    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    // Successo senza valore di ritorno
    public static Result Success() =>
        new(true, null);

    // Fallimento con messaggio
    public static Result Failure(string error) =>
        new(false, error);
}