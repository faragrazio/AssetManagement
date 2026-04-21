using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetManagement.Infrastructure.Persistence.Configurations;

// Configurazione EF Core per la tabella Assets
// Dice a EF Core come mappare la classe Asset al database PostgreSQL
public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        // Nome della tabella nel DB
        builder.ToTable("assets");

        // Chiave primaria con auto-increment
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .UseIdentityColumn(); // PostgreSQL SERIAL/IDENTITY

        // Proprietà Name — obbligatoria, max 200 caratteri
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        // SerialNumber — obbligatorio, max 100 caratteri, indice univoco
        builder.Property(a => a.SerialNumber)
            .IsRequired()
            .HasMaxLength(100);

        // Indice univoco su SerialNumber — nessun duplicato nel DB
        builder.HasIndex(a => a.SerialNumber)
            .IsUnique();

        builder.Property(a => a.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Location)
            .IsRequired()
            .HasMaxLength(200);

        // Status salvato come int nel DB (1=Active, 2=InMaintenance, 3=Decommissioned)
        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.PurchaseDate)
               .IsRequired()
               .HasColumnType("timestamp");

        builder.Property(a => a.CreatedAt)
              .IsRequired()
              .HasColumnType("timestamp");

        builder.Property(a => a.UpdatedAt)
              .IsRequired()
              .HasColumnType("timestamp");
        }
}