using EasyCqrs.Results;
using MediatR;

namespace EasyCqrs;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
