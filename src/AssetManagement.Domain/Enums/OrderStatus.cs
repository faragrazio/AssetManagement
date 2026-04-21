namespace AssetManagement.Domain.Enums;

// Rappresenta il ciclo di vita di un ordine di manutenzione
public enum OrderStatus
{
    Pending = 1,    // Ordine creato, in attesa di presa in carico
    InProgress = 2, // Manutenzione in corso
    Completed = 3,  // Manutenzione completata con successo
    Cancelled = 4   // Ordine annullato
}