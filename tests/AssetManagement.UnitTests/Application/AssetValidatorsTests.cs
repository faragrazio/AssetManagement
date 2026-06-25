using AssetManagement.Application.Assets.Commands.CreateAsset;
using AssetManagement.Application.Assets.Commands.UpdateAsset;
using AssetManagement.Application.Assets.Commands.DeleteAsset;
using FluentAssertions;

namespace AssetManagement.UnitTests.Application;

// Test dei validator FluentValidation per i comandi sugli Asset.
// Teoria veloce: un Validator non tocca il database. Riceve un "command"
// (i dati in ingresso) e dice solo SE sono formalmente validi.
// Lo testiamo chiamando .Validate(command) e guardando .IsValid / .Errors.
public class AssetValidatorsTests
{
    // ── CreateAssetCommandValidator ─────────────────────────────────────

    [Fact]
    public void CreateAsset_ConDatiValidi_Passa()
    {
        var validator = new CreateAssetCommandValidator();
        var command = new CreateAssetCommand(
            Name: "Tornio",
            SerialNumber: "SN-1",
            Category: "Macchinario",
            Location: "Reparto 3",
            PurchaseDate: DateTime.UtcNow.AddDays(-10));

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateAsset_ConNomeVuoto_FallisceSulCampoName()
    {
        var validator = new CreateAssetCommandValidator();
        var command = new CreateAssetCommand("", "SN-1", "Cat", "Loc", DateTime.UtcNow.AddDays(-1));

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        // L'errore deve riguardare proprio il campo "Name".
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void CreateAsset_ConDataNelFuturo_FallisceSulCampoPurchaseDate()
    {
        var validator = new CreateAssetCommandValidator();
        var command = new CreateAssetCommand("Nome", "SN-1", "Cat", "Loc", DateTime.UtcNow.AddDays(1));

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PurchaseDate");
    }

    [Fact]
    public void CreateAsset_ConNomeTroppoLungo_Fallisce()
    {
        var validator = new CreateAssetCommandValidator();
        // 201 caratteri: supera il limite di 200.
        var nomeLungo = new string('a', 201);
        var command = new CreateAssetCommand(nomeLungo, "SN-1", "Cat", "Loc", DateTime.UtcNow.AddDays(-1));

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    // ── UpdateAssetCommandValidator ─────────────────────────────────────

    [Fact]
    public void UpdateAsset_ConDatiValidi_Passa()
    {
        var validator = new UpdateAssetCommandValidator();
        var command = new UpdateAssetCommand(Id: 1, Name: "Nome", Category: "Cat", Location: "Loc");

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdateAsset_ConIdNonValido_FallisceSulCampoId()
    {
        var validator = new UpdateAssetCommandValidator();
        var command = new UpdateAssetCommand(Id: 0, Name: "Nome", Category: "Cat", Location: "Loc");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    // ── DeleteAssetCommandValidator ─────────────────────────────────────

    [Fact]
    public void DeleteAsset_ConIdValido_Passa()
    {
        var validator = new DeleteAssetCommandValidator();
        var command = new DeleteAssetCommand(Id: 5);

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void DeleteAsset_ConIdNonValido_Fallisce()
    {
        var validator = new DeleteAssetCommandValidator();
        var command = new DeleteAssetCommand(Id: 0);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }
}