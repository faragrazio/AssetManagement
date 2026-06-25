using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Exceptions;
using FluentAssertions;

namespace AssetManagement.UnitTests.Domain;

// Test della entità User.
// Punti chiave: validazione anagrafica, email normalizzata in minuscolo,
// soft delete (Deactivate) e aggiornamento profilo/password.
public class UserTests
{
    private static User CreateValidUser() =>
        new(
            firstName: "Mario",
            lastName: "Rossi",
            email: "mario.rossi@example.com",
            passwordHash: "$2a$11$hashfittizioperitest",
            role: "Technician");

    // ── Costruttore ─────────────────────────────────────────────────────

    [Fact]
    public void Costruttore_ConDatiValidi_CreaUtenteAttivo()
    {
        var user = CreateValidUser();

        // Un nuovo utente nasce attivo.
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Costruttore_NormalizzaEmailInMinuscolo()
    {
        // Passo l'email in MAIUSCOLO: il costruttore deve salvarla minuscola.
        var user = new User("Mario", "Rossi", "MARIO.ROSSI@EXAMPLE.COM",
            "$2a$11$hash", "Admin");

        user.Email.Should().Be("mario.rossi@example.com");
    }

    [Fact]
    public void Costruttore_ConNomeVuoto_LanciaDomainException()
    {
        Action act = () => new User("", "Rossi", "m@x.it", "$2a$11$hash", "Admin");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Costruttore_ConCognomeVuoto_LanciaDomainException()
    {
        Action act = () => new User("Mario", "", "m@x.it", "$2a$11$hash", "Admin");

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData("")]              // vuota
    [InlineData("senza-chiocciola")] // manca la @
    public void Costruttore_ConEmailNonValida_LanciaDomainException(string emailNonValida)
    {
        Action act = () => new User("Mario", "Rossi", emailNonValida, "$2a$11$hash", "Admin");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Costruttore_ConPasswordHashVuoto_LanciaDomainException()
    {
        Action act = () => new User("Mario", "Rossi", "m@x.it", "", "Admin");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Costruttore_ConRuoloVuoto_LanciaDomainException()
    {
        Action act = () => new User("Mario", "Rossi", "m@x.it", "$2a$11$hash", "");

        act.Should().Throw<DomainException>();
    }

    // ── Proprietà calcolata FullName ────────────────────────────────────

    [Fact]
    public void FullName_RestituisceNomeESpazioCognome()
    {
        var user = CreateValidUser();

        user.FullName.Should().Be("Mario Rossi");
    }

    // ── Soft delete: Deactivate ─────────────────────────────────────────

    [Fact]
    public void Deactivate_QuandoAttivo_DisattivaUtente()
    {
        var user = CreateValidUser();

        user.Deactivate();

        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Deactivate_QuandoGiaDisattivato_LanciaDomainException()
    {
        var user = CreateValidUser();
        user.Deactivate();

        Action act = () => user.Deactivate();

        act.Should().Throw<DomainException>();
    }

    // ── UpdateProfile ───────────────────────────────────────────────────

    [Fact]
    public void UpdateProfile_ConNomeValido_AggiornaIDati()
    {
        var user = CreateValidUser();

        user.UpdateProfile("Luigi", "Bianchi");

        user.FirstName.Should().Be("Luigi");
        user.LastName.Should().Be("Bianchi");
    }

    [Fact]
    public void UpdateProfile_ConNomeVuoto_LanciaDomainException()
    {
        var user = CreateValidUser();

        Action act = () => user.UpdateProfile("", "Bianchi");

        act.Should().Throw<DomainException>();
    }

    // ── UpdatePasswordHash ──────────────────────────────────────────────

    [Fact]
    public void UpdatePasswordHash_ConValoreValido_AggiornaHash()
    {
        var user = CreateValidUser();

        user.UpdatePasswordHash("$2a$11$nuovohash");

        user.PasswordHash.Should().Be("$2a$11$nuovohash");
    }

    [Fact]
    public void UpdatePasswordHash_ConValoreVuoto_LanciaDomainException()
    {
        var user = CreateValidUser();

        Action act = () => user.UpdatePasswordHash("");

        act.Should().Throw<DomainException>();
    }
}