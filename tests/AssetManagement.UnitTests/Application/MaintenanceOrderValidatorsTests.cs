using AssetManagement.Application.MaintenanceOrders.Commands.CreateOrder;
using AssetManagement.Application.MaintenanceOrders.Commands.UpdateOrderStatus;
using AssetManagement.Domain.Enums;
using FluentAssertions;

namespace AssetManagement.UnitTests.Application;

// Test dei validator per gli ordini di manutenzione.
// Stessa idea degli altri: il validator NON tocca il database,
// controlla solo se i dati in ingresso (il "command") sono formalmente validi.
public class MaintenanceOrderValidatorsTests
{
    // ── CreateOrderCommandValidator ─────────────────────────────────────

    // Piccola "fabbrica" che crea un comando valido di partenza,
    // così in ogni test cambiamo SOLO il campo che ci interessa controllare.
    private static CreateOrderCommand ComandoValido() => new(
        AssetId: 1,
        Title: "Sostituzione cuscinetti",
        Description: "Rumore anomalo sul mandrino, sostituire i cuscinetti.",
        Priority: Priority.High,
        AssignedTo: "Mario Rossi",
        ScheduledDate: DateTime.UtcNow.AddDays(2));

    [Fact]
    public void CreateOrder_ConDatiValidi_Passa()
    {
        var validator = new CreateOrderCommandValidator();

        var result = validator.Validate(ComandoValido());

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateOrder_ConAssetIdNonValido_FallisceSulCampoAssetId()
    {
        var validator = new CreateOrderCommandValidator();
        var command = ComandoValido() with { AssetId = 0 };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AssetId");
    }

    [Fact]
    public void CreateOrder_ConTitoloVuoto_FallisceSulCampoTitle()
    {
        var validator = new CreateOrderCommandValidator();
        var command = ComandoValido() with { Title = "" };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void CreateOrder_ConTitoloTroppoLungo_Fallisce()
    {
        var validator = new CreateOrderCommandValidator();
        // 201 caratteri: supera il limite di 200.
        var command = ComandoValido() with { Title = new string('a', 201) };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateOrder_ConDescrizioneVuota_FallisceSulCampoDescription()
    {
        var validator = new CreateOrderCommandValidator();
        var command = ComandoValido() with { Description = "" };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void CreateOrder_ConTecnicoNonAssegnato_FallisceSulCampoAssignedTo()
    {
        var validator = new CreateOrderCommandValidator();
        var command = ComandoValido() with { AssignedTo = "" };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AssignedTo");
    }

    [Fact]
    public void CreateOrder_SenzaDataPianificata_FallisceSulCampoScheduledDate()
    {
        var validator = new CreateOrderCommandValidator();
        // default(DateTime) è una data "vuota": la regola NotEmpty la rifiuta.
        var command = ComandoValido() with { ScheduledDate = default };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ScheduledDate");
    }

    [Fact]
    public void CreateOrder_ConPrioritaInesistente_FallisceSulCampoPriority()
    {
        var validator = new CreateOrderCommandValidator();
        // 999 non corrisponde a nessun valore dell'enum Priority.
        var command = ComandoValido() with { Priority = (Priority)999 };

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Priority");
    }

    // ── UpdateOrderStatusCommandValidator ───────────────────────────────

    [Fact]
    public void UpdateStatus_ConDatiValidi_Passa()
    {
        var validator = new UpdateOrderStatusCommandValidator();
        var command = new UpdateOrderStatusCommand(OrderId: 1, NewStatus: OrderStatus.InProgress);

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdateStatus_VersoCompletatoConNote_Passa()
    {
        var validator = new UpdateOrderStatusCommandValidator();
        var command = new UpdateOrderStatusCommand(
            OrderId: 1,
            NewStatus: OrderStatus.Completed,
            CompletionNotes: "Intervento eseguito, macchina ripristinata.");

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void UpdateStatus_ConOrderIdNonValido_FallisceSulCampoOrderId()
    {
        var validator = new UpdateOrderStatusCommandValidator();
        var command = new UpdateOrderStatusCommand(OrderId: 0, NewStatus: OrderStatus.InProgress);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
    }

    [Fact]
    public void UpdateStatus_VersoPending_FallisceSulCampoNewStatus()
    {
        var validator = new UpdateOrderStatusCommandValidator();
        // Pending è lo stato iniziale: non si può reimpostare a mano.
        var command = new UpdateOrderStatusCommand(OrderId: 1, NewStatus: OrderStatus.Pending);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NewStatus");
    }

    [Fact]
    public void UpdateStatus_ConStatoInesistente_FallisceSulCampoNewStatus()
    {
        var validator = new UpdateOrderStatusCommandValidator();
        var command = new UpdateOrderStatusCommand(OrderId: 1, NewStatus: (OrderStatus)999);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NewStatus");
    }

    [Fact]
    public void UpdateStatus_ConNoteTroppoLunghe_FallisceSulCampoCompletionNotes()
    {
        var validator = new UpdateOrderStatusCommandValidator();
        // 1001 caratteri: supera il limite di 1000.
        var command = new UpdateOrderStatusCommand(
            OrderId: 1,
            NewStatus: OrderStatus.Completed,
            CompletionNotes: new string('a', 1001));

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CompletionNotes");
    }
}
