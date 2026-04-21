using FluentValidation;
using MediatR;

namespace AssetManagement.Application.Common.Behaviors;

// Behavior MediatR che intercetta ogni request e la valida prima dell'Handler
// TRequest = tipo del Command o Query
// TResponse = tipo della risposta
public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    // Lista di tutti i validator registrati per questo tipo di request
    // Iniettati dal DI container — se non esistono validator, la lista è vuota
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Se non ci sono validator registrati per questa request, procedi direttamente
        if (!_validators.Any())
            return await next();

        // Crea il contesto di validazione con la request corrente
        var context = new ValidationContext<TRequest>(request);

        // Esegui tutti i validator in parallelo e raccogli i risultati
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        // Raccogli tutti gli errori da tutti i validator
        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .ToList();

        // Se ci sono errori, lancia ValidationException — blocca l'Handler
        if (failures.Count != 0)
            throw new ValidationException(failures);

        // Nessun errore — procedi con il prossimo behavior o con l'Handler
        return await next();
    }
}