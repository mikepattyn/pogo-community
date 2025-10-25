using MediatR;
using Microsoft.Extensions.Logging;
using Pogo.Shared.Kernel;

namespace Pogo.Shared.Application;

/// <summary>
/// Base class for command handlers
/// </summary>
/// <typeparam name="TCommand">The command type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public abstract class CommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
    protected readonly ILogger<CommandHandler<TCommand, TResponse>> _logger;

    protected CommandHandler(ILogger<CommandHandler<TCommand, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<Result<TResponse>> Handle(TCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling command {CommandType}", typeof(TCommand).Name);
            
            var result = await HandleCommand(request, cancellationToken);
            
            _logger.LogInformation("Command {CommandType} handled successfully", typeof(TCommand).Name);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling command {CommandType}", typeof(TCommand).Name);
            return Result<TResponse>.Failure($"An error occurred while processing the command: {ex.Message}");
        }
    }

    protected abstract Task<Result<TResponse>> HandleCommand(TCommand request, CancellationToken cancellationToken);
}

/// <summary>
/// Base class for command handlers that don't return a value
/// </summary>
/// <typeparam name="TCommand">The command type</typeparam>
public abstract class CommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
    protected readonly ILogger<CommandHandler<TCommand>> _logger;

    protected CommandHandler(ILogger<CommandHandler<TCommand>> logger)
    {
        _logger = logger;
    }

    public async Task<Result> Handle(TCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling command {CommandType}", typeof(TCommand).Name);
            
            var result = await HandleCommand(request, cancellationToken);
            
            _logger.LogInformation("Command {CommandType} handled successfully", typeof(TCommand).Name);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling command {CommandType}", typeof(TCommand).Name);
            return Result.Failure($"An error occurred while processing the command: {ex.Message}");
        }
    }

    protected abstract Task<Result> HandleCommand(TCommand request, CancellationToken cancellationToken);
}
