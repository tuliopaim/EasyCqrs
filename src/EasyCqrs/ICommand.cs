using EasyCqrs.Results;
using MediatR;

namespace EasyCqrs;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
