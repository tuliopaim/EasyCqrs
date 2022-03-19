using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Queries.GetPeopleQuery;

public class GetPeopleQueryHandler : IQueryHandler<GetPeopleQueryInput, GetPeopleQueryResult>
{
    public Task<GetPeopleQueryResult> Handle(GetPeopleQueryInput request, CancellationToken cancellationToken)
    {
        var list = new List<GetPeopleResult> { new(), new() };
        return Task.FromResult(new GetPeopleQueryResult
        {
            Results = list,
            Pagination = new QueryPagination
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalElements = 2
            }
        });

    }
}