namespace AssetManagement.Domain.Interfaces;

// Interfaccia generica base — definisce le operazioni CRUD comuni a tutte le entità
// T deve essere una classe (not null)
public interface IRepository<T> where T : class
{
    // Recupera un'entità per ID — restituisce null se non esiste
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Recupera tutte le entità — usare con cautela su tabelle grandi
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    // Aggiunge una nuova entità al contesto (non salva ancora nel DB)
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    // Marca un'entità come modificata nel contesto EF Core
    void Update(T entity);

    // Marca un'entità per l'eliminazione
    void Remove(T entity);

    // Persiste tutte le modifiche pendenti nel DB
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}