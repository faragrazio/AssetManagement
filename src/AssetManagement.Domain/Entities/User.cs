using AssetManagement.Domain.Exceptions;

namespace AssetManagement.Domain.Entities;

// Rappresenta un utente del sistema (tecnico, responsabile, admin)
public class User
{
    public int Id { get; private set; }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    // Hash BCrypt della password — mai la password in chiaro
    public string PasswordHash { get; private set; } = string.Empty;

    // Ruolo dell'utente nel sistema
    public string Role { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Costruttore privato per EF Core
    private User() { }

    public User(string firstName, string lastName, string email,
        string passwordHash, string role)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("Il nome non può essere vuoto.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Il cognome non può essere vuoto.");

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("Email non valida.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Il password hash non può essere vuoto.");

        if (string.IsNullOrWhiteSpace(role))
            throw new DomainException("Il ruolo non può essere vuoto.");

        FirstName = firstName;
        LastName = lastName;
        Email = email.ToLowerInvariant(); // Email sempre in minuscolo
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Proprietà calcolata — non salvata nel DB, utile nei DTO e nei log
    public string FullName => $"{FirstName} {LastName}";

    // Disattiva l'utente senza eliminarlo (soft delete)
    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("L'utente è già disattivato.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    // Aggiorna i dati anagrafici
    public void UpdateProfile(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("Il nome non può essere vuoto.");

        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    // Aggiorna il password hash dopo un cambio password
    public void UpdatePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("Il password hash non può essere vuoto.");

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }
}