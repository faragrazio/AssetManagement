using System.Net;
using System.Net.Http.Json;

namespace AssetManagement.IntegrationTests;

// Test di INTEGRAZIONE sugli endpoint di autenticazione.
// A differenza degli unit test, qui gira l'app vera + database vero:
// inviamo richieste HTTP reali e controlliamo le risposte.
//
// IClassFixture<CustomWebApplicationFactory> dice a xUnit:
// "crea la factory (e quindi il container) una sola volta per questa classe".
public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        // HttpClient che parla con l'app in memoria.
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ConDatiValidi_RestituisceCreated()
    {
        // Email casuale così il test non collide con altri dati.
        var nuovoUtente = new
        {
            firstName = "Mario",
            lastName = "Rossi",
            email = $"mario.{Guid.NewGuid():N}@example.com",
            password = "Password1",
            role = "Technician"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", nuovoUtente);

        // Registrazione andata a buon fine => 201 Created.
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Register_PoiLogin_RestituisceOk()
    {
        var email = $"luigi.{Guid.NewGuid():N}@example.com";

        // 1. Registriamo l'utente.
        var registrazione = new
        {
            firstName = "Luigi",
            lastName = "Verdi",
            email,
            password = "Password1",
            role = "Technician"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registrazione);

        // 2. Facciamo login con le stesse credenziali.
        var login = new { email, password = "Password1" };
        var response = await _client.PostAsJsonAsync("/api/auth/login", login);

        // Login riuscito => 200 OK e un corpo non vuoto (contiene il token).
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(body));
    }

    [Fact]
    public async Task Register_ConEmailDuplicata_RestituisceBadRequest()
    {
        var email = $"dup.{Guid.NewGuid():N}@example.com";
        var utente = new
        {
            firstName = "Anna",
            lastName = "Bianchi",
            email,
            password = "Password1",
            role = "Technician"
        };

        // Prima registrazione: ok.
        await _client.PostAsJsonAsync("/api/auth/register", utente);
        // Seconda con la stessa email: l'handler restituisce un errore => 400.
        var response = await _client.PostAsJsonAsync("/api/auth/register", utente);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
