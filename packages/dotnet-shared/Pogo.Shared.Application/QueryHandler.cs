using MediatR;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Kernel;

namespace Pogo.Shared.Application;

/// <summary>
/// Base class for query handlers
/// </summary>
/// <typeparam name="TQuery">The query type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public abstract class QueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
    protected readonly ILogger<QueryHandler<TQuery, TResponse>> _logger;

    protected QueryHandler(ILogger<QueryHandler<TQuery, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(TQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling query {QueryType}", typeof(TQuery).Name);
            
            var result = await HandleQuery(request, cancellationToken);
            
            _logger.LogInformation("Query {QueryType} handled successfully", typeof(TQuery).Name);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling query {QueryType}", typeof(TQuery).Name);
            return Result<TResponse>.Failure($"An error occurred while processing the query: {ex.Message}");
        }
    }

    protected abstract Task<Result<TResponse>> HandleQuery(TQuery request, CancellationToken cancellationToken);
}
