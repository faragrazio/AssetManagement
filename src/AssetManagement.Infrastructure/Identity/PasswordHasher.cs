using AssetManagement.Application.Common.Interfaces;

namespace AssetManagement.Infrastructure.Identity;

// Implementazione concreta di IPasswordHasher usando BCrypt
public class PasswordHasher : IPasswordHasher
{
    // Genera un hash BCrypt dalla password in chiaro
    // WorkFactor 12 = buon equilibrio tra sicurezza e performance
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    // Verifica che la password in chiaro corrisponda all'hash salvato nel DB
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}