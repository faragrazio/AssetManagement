using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.Common.Interfaces;

// Contratto per la generazione e validazione dei token JWT
public interface IJwtService
{
    // Genera un token JWT per l'utente autenticato
    string GenerateToken(User user);

    // Valida un token e restituisce l'email dell'utente — null se non valido
    string? ValidateToken(string token);

    // Restituisce la durata del token in ore dalla configurazione
    int GetExpiryHours();
}