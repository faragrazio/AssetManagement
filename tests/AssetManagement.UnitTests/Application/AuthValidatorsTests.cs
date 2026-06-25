using AssetManagement.Application.Auth.Commands.Register;
using AssetManagement.Application.Auth.Commands.Login;
using FluentAssertions;

namespace AssetManagement.UnitTests.Application;

// Test dei validator per la registrazione e il login.
// Qui le regole interessanti sono: formato email, robustezza password e ruoli ammessi.
public class AuthValidatorsTests
{
    // ── RegisterCommandValidator ────────────────────────────────────────

    [Fact]
    public void Register_ConDatiValidi_Passa()
    {
        var validator = new RegisterCommandValidator();
        var command = new RegisterCommand(
            FirstName: "Mario",
            LastName: "Rossi",
            Email: "mario.rossi@example.com",
            Password: "Password1",   // >=8, una maiuscola, un numero
            Role: "Technician");     // ruolo ammesso

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Register_ConEmailNonValida_FallisceSulCampoEmail()
    {
        var validator = new RegisterCommandValidator();
        var command = new RegisterCommand("Mario", "Rossi", "email-non-valida", "Password1", "Technician");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("corta1")]      // meno di 8 caratteri
    [InlineData("password1")]   // manca la maiuscola
    [InlineData("Password")]    // manca il numero
    public void Register_ConPasswordDebole_FallisceSulCampoPassword(string passwordDebole)
    {
        var validator = new RegisterCommandValidator();
        var command = new RegisterCommand("Mario", "Rossi", "m@x.it", passwordDebole, "Technician");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Register_ConRuoloNonAmmesso_FallisceSulCampoRole()
    {
        var validator = new RegisterCommandValidator();
        // "SuperAdmin" non è tra i ruoli ammessi (Admin, Technician, Viewer).
        var command = new RegisterCommand("Mario", "Rossi", "m@x.it", "Password1", "SuperAdmin");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Role");
    }

    // ── LoginCommandValidator ───────────────────────────────────────────

    [Fact]
    public void Login_ConDatiValidi_Passa()
    {
        var validator = new LoginCommandValidator();
        var command = new LoginCommand(Email: "mario@example.com", Password: "secret1");

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Login_ConEmailVuota_Fallisce()
    {
        var validator = new LoginCommandValidator();
        var command = new LoginCommand(Email: "", Password: "secret1");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Login_ConPasswordTroppoCorta_FallisceSulCampoPassword()
    {
        var validator = new LoginCommandValidator();
        // 5 caratteri: sotto il minimo di 6.
        var command = new LoginCommand(Email: "mario@example.com", Password: "12345");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}