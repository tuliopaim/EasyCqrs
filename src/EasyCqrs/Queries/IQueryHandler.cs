using MediatR;

namespace EasyCqrs.Queries;

public interface IQueryHandler<in TQueryInput, TItem> : IRequestHandler<TQueryInput, TItem>
    where TQueryInput : IQueryInput<TItem>
{
}