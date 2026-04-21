using AssetManagement.Domain.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace AssetManagement.API.Middleware;

// Middleware che intercetta tutte le eccezioni non gestite
// e le converte in risposte HTTP con il codice corretto
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Passa la request al prossimo middleware/controller
            await _next(context);
        }
        catch (Exception ex)
        {
            // Intercetta qualsiasi eccezione non gestita
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            // Errori di validazione FluentValidation → 400 Bad Request
            ValidationException validationEx =>
                (HttpStatusCode.BadRequest,
                 string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage))),

            // Risorsa non trovata → 404 Not Found
            NotFoundException notFoundEx =>
                (HttpStatusCode.NotFound, notFoundEx.Message),

            // Violazione regola di business → 400 Bad Request
            DomainException domainEx =>
                (HttpStatusCode.BadRequest, domainEx.Message),

            // Qualsiasi altra eccezione → 500 Internal Server Error
            _ => (HttpStatusCode.InternalServerError,
                  "Si è verificato un errore interno del server.")
        };

        // Log dell'errore — per 500 logga anche lo stack trace
        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Errore interno: {Message}", exception.Message);
        else
            _logger.LogWarning("Errore gestito: {Message}", exception.Message);

        // Costruisce la risposta JSON standardizzata
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            error = message,
            timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}