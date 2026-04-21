namespace AssetManagement.Domain.Enums;

// Priorità di un ordine di manutenzione
public enum Priority
{
    Low = 1,      // Manutenzione programmata, non urgente
    Medium = 2,   // Intervento da pianificare a breve
    High = 3,     // Intervento urgente
    Critical = 4  // Fermo macchina, intervento immediato richiesto
}