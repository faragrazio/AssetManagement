using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Exceptions;
using FluentAssertions;

namespace AssetManagement.UnitTests.Domain;

// Test della entità MaintenanceOrder (ordine di manutenzione).
// Il cuore qui è il ciclo di vita: Pending -> InProgress -> Completed,
// con la possibilità di Cancel da Pending o InProgress (ma non da Completed).
public class MaintenanceOrderTests
{
    // Helper: ordine valido. ScheduledDate nel futuro per superare la validazione del costruttore.
    private static MaintenanceOrder CreateValidOrder() =>
        new(
            assetId: 1,
            title: "Sostituzione cinghia",
            description: "La cinghia del motore mostra usura",
            priority: Priority.High,
            assignedTo: "Mario Rossi",
            scheduledDate: DateTime.UtcNow.AddDays(1));

    // ── Costruttore ─────────────────────────────────────────────────────

    [Fact]
    public void Costruttore_ConDatiValidi_CreaOrdineInStatoPending()
    {
        var order = CreateValidOrder();

        // Ogni nuovo ordine deve partire da Pending (in attesa di presa in carico).
        order.Status.Should().Be(OrderStatus.Pending);
        order.AssetId.Should().Be(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Costruttore_ConAssetIdNonValido_LanciaDomainException(int assetIdNonValido)
    {
        Action act = () => new MaintenanceOrder(
            assetIdNonValido, "Titolo", "Descrizione", Priority.Low, "Tecnico", DateTime.UtcNow.AddDays(1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Costruttore_ConTitoloVuoto_LanciaDomainException()
    {
        Action act = () => new MaintenanceOrder(
            1, "", "Descrizione", Priority.Low, "Tecnico", DateTime.UtcNow.AddDays(1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Costruttore_ConDescrizioneVuota_LanciaDomainException()
    {
        Action act = () => new MaintenanceOrder(
            1, "Titolo", "", Priority.Low, "Tecnico", DateTime.UtcNow.AddDays(1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Costruttore_ConTecnicoVuoto_LanciaDomainException()
    {
        Action act = () => new MaintenanceOrder(
            1, "Titolo", "Descrizione", Priority.Low, "", DateTime.UtcNow.AddDays(1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Costruttore_ConDataNelPassato_LanciaDomainException()
    {
        // Pianificare un intervento nel passato non ha senso.
        Action act = () => new MaintenanceOrder(
            1, "Titolo", "Descrizione", Priority.Low, "Tecnico", DateTime.UtcNow.AddDays(-1));

        act.Should().Throw<DomainException>();
    }

    // ── Start ───────────────────────────────────────────────────────────

    [Fact]
    public void Start_QuandoPending_PortaInProgress()
    {
        var order = CreateValidOrder();

        order.Start();

        order.Status.Should().Be(OrderStatus.InProgress);
    }

    [Fact]
    public void Start_QuandoNonPending_LanciaDomainException()
    {
        var order = CreateValidOrder();
        order.Start(); // ora è InProgress

        // Non si può avviare un ordine già avviato.
        Action act = () => order.Start();

        act.Should().Throw<DomainException>();
    }

    // ── Complete ────────────────────────────────────────────────────────

    [Fact]
    public void Complete_QuandoInProgress_CompletaEValorizzaCompletedAt()
    {
        var order = CreateValidOrder();
        order.Start();

        order.Complete("Intervento eseguito, cinghia sostituita");

        order.Status.Should().Be(OrderStatus.Completed);
        // Al completamento devono comparire data e note.
        order.CompletedAt.Should().NotBeNull();
        order.CompletionNotes.Should().Be("Intervento eseguito, cinghia sostituita");
    }

    [Fact]
    public void Complete_QuandoAncoraPending_LanciaDomainException()
    {
        var order = CreateValidOrder(); // è Pending, non InProgress

        Action act = () => order.Complete();

        act.Should().Throw<DomainException>();
    }

    // ── Cancel ──────────────────────────────────────────────────────────

    [Fact]
    public void Cancel_QuandoPending_AnnullaOrdine()
    {
        var order = CreateValidOrder();

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_QuandoInProgress_AnnullaOrdine()
    {
        var order = CreateValidOrder();
        order.Start();

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_QuandoCompletato_LanciaDomainException()
    {
        var order = CreateValidOrder();
        order.Start();
        order.Complete();

        // Un ordine completato non si può annullare.
        Action act = () => order.Cancel();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_QuandoGiaAnnullato_LanciaDomainException()
    {
        var order = CreateValidOrder();
        order.Cancel();

        Action act = () => order.Cancel();

        act.Should().Throw<DomainException>();
    }
}