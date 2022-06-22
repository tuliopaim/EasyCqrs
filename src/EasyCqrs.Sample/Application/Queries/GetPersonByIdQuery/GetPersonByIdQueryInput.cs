using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.GetPersonByIdQuery;

public class GetPersonByIdQueryInput : QueryInput<QueryResult<GetPersonByIdItem>>
{
    public GetPersonByIdQueryInput(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}