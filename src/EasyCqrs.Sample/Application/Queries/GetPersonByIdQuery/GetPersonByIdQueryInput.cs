using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.GetPersonByIdQuery;

public record GetPersonByIdQueryInput(Guid Id) : IQueryInput<QueryResult<GetPersonByIdQueryItem>>
{
}
