using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Kernel;

namespace Pogo.Shared.Application;

/// <summary>
/// Pipeline behavior for validating requests using FluentValidation
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            _logger.LogWarning("Validation failed for {RequestType}: {Failures}", 
                typeof(TRequest).Name, string.Join(", ", failures.Select(f => f.ErrorMessage)));

            if (typeof(TResponse).IsGenericType && 
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Result<>).MakeGenericType(resultType).GetMethod("Failure", new[] { typeof(string) });
                var errorMessage = string.Join(", ", failures.Select(f => f.ErrorMessage));
                return (TResponse)Activator.CreateInstance(typeof(TResponse), false, null, errorMessage)!;
            }
            else if (typeof(TResponse) == typeof(Result))
            {
                var errorMessage = string.Join(", ", failures.Select(f => f.ErrorMessage));
                return (TResponse)(object)Result.Failure(errorMessage);
            }
        }

        return await next();
    }
}
