using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetManagement.Infrastructure.Persistence.Configurations;

public class MaintenanceOrderConfiguration : IEntityTypeConfiguration<MaintenanceOrder>
{
    public void Configure(EntityTypeBuilder<MaintenanceOrder> builder)
    {
        builder.ToTable("maintenance_orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .UseIdentityColumn();

        builder.Property(o => o.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(o => o.AssignedTo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.ScheduledDate)
               .IsRequired()
               .HasColumnType("timestamp");

        // CompletedAt e CompletionNotes sono nullable — non richiedono IsRequired()
        builder.Property(o => o.CompletedAt)
               .HasColumnType("timestamp");       
        builder.Property(o => o.CompletionNotes)
               .HasMaxLength(1000);

        builder.Property(o => o.CreatedAt).IsRequired().HasColumnType("timestamp");
        builder.Property(o => o.UpdatedAt).IsRequired().HasColumnType("timestamp");

        // Relazione MaintenanceOrder → Asset (molti a uno)
        // Un asset può avere molti ordini, un ordine appartiene a un solo asset
        builder.HasOne(o => o.Asset)
            .WithMany()
            .HasForeignKey(o => o.AssetId)
            .OnDelete(DeleteBehavior.Restrict); // non eliminare gli ordini se elimini l'asset
    }
}