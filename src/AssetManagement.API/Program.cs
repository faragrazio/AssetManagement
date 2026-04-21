using AssetManagement.API.Extensions;
using AssetManagement.API.Middleware;

// Configura Npgsql per accettare DateTime con Kind=Unspecified trattandoli come UTC
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// ── Registrazione servizi ─────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwagger();

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────
// Ordine importante: ExceptionHandling deve essere il primo
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // prima di Authorization
app.UseAuthorization();
app.MapControllers();

app.Run();

// Rende Program accessibile ai test di integrazione
public partial class Program { }