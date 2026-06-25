using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Exceptions;
using FluentAssertions;

namespace AssetManagement.UnitTests.Domain;

// Test della entità Asset.
// Qui NON tocchiamo database o rete: creiamo oggetti in memoria e verifichiamo
// che le REGOLE DI BUSINESS (invarianti + macchina a stati) si comportino come previsto.
public class AssetTests
{
    // Metodo "helper": crea un Asset valido di default.
    // Evita di ripetere gli stessi argomenti in ogni test (principio DRY).
    private static Asset CreateValidAsset() =>
        new(
            name: "Tornio CNC",
            serialNumber: "SN-12345",
            category: "Macchinario",
            location: "Reparto 3",
            purchaseDate: DateTime.UtcNow.AddDays(-30));

    // ── Costruttore: caso felice ────────────────────────────────────────

    [Fact] // [Fact] = un test senza parametri, eseguito una volta sola
    public void Costruttore_ConDatiValidi_CreaAssetInStatoActive()
    {
        // Arrange + Act: creo l'asset
        var asset = CreateValidAsset();

        // Assert: un asset appena creato deve partire Active (lo dice il costruttore)
        asset.Status.Should().Be(AssetStatus.Active);
        asset.Name.Should().Be("Tornio CNC");
    }

    [Fact]
    public void Costruttore_ImpostaLeDateDiCreazione()
    {
        var asset = CreateValidAsset();

        // CreatedAt non deve essere il valore di default (DateTime non inizializzato)
        asset.CreatedAt.Should().NotBe(default);
        asset.UpdatedAt.Should().NotBe(default);
    }

    // ── Costruttore: casi di errore ─────────────────────────────────────

    // [Theory] = stesso test eseguito più volte con dati diversi (InlineData).
    // Così copro "stringa vuota", "spazi" e null con UN solo metodo.
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Costruttore_ConNomeVuoto_LanciaDomainException(string? nomeNonValido)
    {
        // "Action" incapsula il codice che DEVE lanciare un'eccezione.
        // Non lo eseguiamo subito: lo passiamo a FluentAssertions che lo invoca e controlla.
        Action act = () => new Asset(nomeNonValido!, "SN-1", "Cat", "Loc", DateTime.UtcNow.AddDays(-1));

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Costruttore_ConSerialeVuoto_LanciaDomainException(string serialeNonValido)
    {
        Action act = () => new Asset("Nome", serialeNonValido, "Cat", "Loc", DateTime.UtcNow.AddDays(-1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Costruttore_ConCategoriaVuota_LanciaDomainException()
    {
        Action act = () => new Asset("Nome", "SN-1", "", "Loc", DateTime.UtcNow.AddDays(-1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Costruttore_ConPosizioneVuota_LanciaDomainException()
    {
        Action act = () => new Asset("Nome", "SN-1", "Cat", "", DateTime.UtcNow.AddDays(-1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Costruttore_ConDataAcquistoNelFuturo_LanciaDomainException()
    {
        // La data di acquisto nel futuro non ha senso: il costruttore deve rifiutarla.
        Action act = () => new Asset("Nome", "SN-1", "Cat", "Loc", DateTime.UtcNow.AddDays(1));

        // Esempio di assert sul MESSAGGIO con wildcard (*): utile, ma rende il test
        // più fragile se cambi il testo. Per questo lo uso solo qui come dimostrazione.
        act.Should().Throw<DomainException>()
            .WithMessage("*futuro*");
    }

    // ── Macchina a stati: StartMaintenance / CompleteMaintenance ────────

    [Fact]
    public void StartMaintenance_QuandoAttivo_PortaInManutenzione()
    {
        var asset = CreateValidAsset(); // parte Active

        asset.StartMaintenance();

        asset.Status.Should().Be(AssetStatus.InMaintenance);
    }

    [Fact]
    public void StartMaintenance_QuandoGiaInManutenzione_LanciaDomainException()
    {
        var asset = CreateValidAsset();
        asset.StartMaintenance(); // ora è InMaintenance

        // Non puoi avviare la manutenzione due volte: lo stato non è più Active.
        Action act = () => asset.StartMaintenance();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CompleteMaintenance_QuandoInManutenzione_TornaActive()
    {
        var asset = CreateValidAsset();
        asset.StartMaintenance();

        asset.CompleteMaintenance();

        asset.Status.Should().Be(AssetStatus.Active);
    }

    [Fact]
    public void CompleteMaintenance_QuandoNonInManutenzione_LanciaDomainException()
    {
        var asset = CreateValidAsset(); // è Active, non InMaintenance

        Action act = () => asset.CompleteMaintenance();

        act.Should().Throw<DomainException>();
    }

    // ── Macchina a stati: Decommission ──────────────────────────────────

    [Fact]
    public void Decommission_QuandoAttivo_DismetteAsset()
    {
        var asset = CreateValidAsset();

        asset.Decommission();

        asset.Status.Should().Be(AssetStatus.Decommissioned);
    }

    [Fact]
    public void Decommission_QuandoGiaDismesso_LanciaDomainException()
    {
        var asset = CreateValidAsset();
        asset.Decommission();

        // Dismettere due volte non è consentito.
        Action act = () => asset.Decommission();

        act.Should().Throw<DomainException>();
    }

    // ── Update ──────────────────────────────────────────────────────────

    [Fact]
    public void Update_ConNomeValido_AggiornaICampi()
    {
        var asset = CreateValidAsset();

        asset.Update(name: "Nuovo Nome", category: "Nuova Categoria", location: "Nuova Posizione");

        asset.Name.Should().Be("Nuovo Nome");
        asset.Category.Should().Be("Nuova Categoria");
        asset.Location.Should().Be("Nuova Posizione");
    }

    [Fact]
    public void Update_ConNomeVuoto_LanciaDomainException()
    {
        var asset = CreateValidAsset();

        Action act = () => asset.Update("", "Cat", "Loc");

        act.Should().Throw<DomainException>();
    }
}