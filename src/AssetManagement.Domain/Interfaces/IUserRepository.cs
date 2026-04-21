using AssetManagement.Domain.Entities;

namespace AssetManagement.Domain.Interfaces;

// Interfaccia specifica per User
public interface IUserRepository : IRepository<User>
{
    // Recupera un utente per email — usato nel login
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    // Verifica se esiste già un utente con quella email
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}