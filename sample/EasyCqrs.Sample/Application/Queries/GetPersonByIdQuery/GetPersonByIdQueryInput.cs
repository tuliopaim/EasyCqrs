using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.GetPersonByIdQuery;

public class GetPersonByIdQueryInput : QueryInput<QueryResult<GetPersonByIdResult>>
{
    public Guid Id { get; set; }
}