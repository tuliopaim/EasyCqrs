using EasyCqrs.Results;
using MediatR;

namespace EasyCqrs;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
