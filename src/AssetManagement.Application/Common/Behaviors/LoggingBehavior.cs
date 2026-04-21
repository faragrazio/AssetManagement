using MediatR;
using Microsoft.Extensions.Logging;

namespace AssetManagement.Application.Common.Behaviors;

// Behavior che logga automaticamente ogni request con il suo tempo di esecuzione
public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Nome della request (es. "CreateAssetCommand")
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Inizio esecuzione: {RequestName}", requestName);

        var startTime = DateTime.UtcNow;

        try
        {
            var response = await next();

            var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogInformation(
                "Completato: {RequestName} in {ElapsedMs}ms",
                requestName, elapsed);

            return response;
        }
        catch (Exception ex)
        {
            var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;
            _logger.LogError(ex,
                "Errore in: {RequestName} dopo {ElapsedMs}ms",
                requestName, elapsed);

            throw; // rilancia l'eccezione — non la ingoia
        }
    }
}