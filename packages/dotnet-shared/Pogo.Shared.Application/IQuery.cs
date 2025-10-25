using MediatR;
using Pogo.Shared.Kernel;

namespace Pogo.Shared.Application;

/// <summary>
/// Base interface for queries that return a result
/// </summary>
/// <typeparam name="TResponse">The type of the response</typeparam>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
