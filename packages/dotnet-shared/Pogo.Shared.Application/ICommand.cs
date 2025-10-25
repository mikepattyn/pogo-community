using MediatR;
using Pogo.Shared.Kernel;

namespace Pogo.Shared.Application;

/// <summary>
/// Base interface for commands that return a result
/// </summary>
/// <typeparam name="TResponse">The type of the response</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

/// <summary>
/// Base interface for commands that don't return a value
/// </summary>
public interface ICommand : IRequest<Result>
{
}
