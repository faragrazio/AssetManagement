using AssetManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace AssetManagement.IntegrationTests;

// Questa classe avvia l'INTERA applicazione in memoria per i test,
// ma collegata a un database PostgreSQL VERO e usa-e-getta, creato
// al volo dentro un container Docker (grazie alla libreria Testcontainers).
//
// In pratica: a ogni esecuzione parte un Postgres pulito, l'app ci si
// collega, eseguiamo i test, e alla fine il container viene buttato via.
// Serve solo che Docker Desktop sia avviato sul PC.
public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Definizione del container PostgreSQL (immagine ufficiale leggera).
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:16-alpine")
    .WithDatabase("assetmanagement_test")
    .WithUsername("postgres")
    .WithPassword("postgres")
    .Build();

    // Qui "sovrascriviamo" la configurazione dell'app SOLO per i test:
    // - la stringa di connessione punta al container appena creato
    // - mettiamo valori JWT finti (servono solo perché l'app riesca a partire)
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _dbContainer.GetConnectionString(),
                ["Jwt:Key"] = "chiave-di-test-lunghissima-solo-per-firmare-i-token-1234567890",
                ["Jwt:Issuer"] = "AssetManagement.Tests",
                ["Jwt:Audience"] = "AssetManagement.Tests"
            });
        });
    }

    // Eseguito UNA volta PRIMA dei test: avvia il container e crea le tabelle.
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        // Creiamo lo schema (le tabelle) sul database del container.
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AssetManagementDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    // Eseguito UNA volta DOPO i test: spegne e cancella il container.
    // Implementazione esplicita per non confliggere con il DisposeAsync della classe base.
    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
