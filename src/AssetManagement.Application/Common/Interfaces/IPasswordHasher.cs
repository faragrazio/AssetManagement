namespace AssetManagement.Application.Common.Interfaces;

// Contratto per l'hashing e la verifica delle password
public interface IPasswordHasher
{
    // Genera un hash BCrypt dalla password in chiaro
    string Hash(string password);

    // Verifica se una password in chiaro corrisponde all'hash salvato
    bool Verify(string password, string hash);
}