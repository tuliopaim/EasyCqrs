using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.GetPersonByIdQuery;

public class GetPersonByIdQueryHandler : IQueryHandler<GetPersonByIdQueryInput, QueryResult<GetPersonByIdResult>>
{
    public Task<QueryResult<GetPersonByIdResult>> Handle(GetPersonByIdQueryInput request, CancellationToken cancellationToken)
    {
        //get the result from your data source...

        var personResult = new GetPersonByIdResult(request.Id, "Person 1", 24);

        return Task.FromResult(new QueryResult<GetPersonByIdResult>
        {
            Result = personResult
        });
    }
}