using EasyCqrs.Queries;

namespace EasyCqrs.Sample.Application.Queries.GetPeopleQuery;

public class GetPeopleQueryHandler : IQueryHandler<GetPeopleQueryInput, GetPeopleQueryResult>
{
    public Task<GetPeopleQueryResult> Handle(GetPeopleQueryInput request, CancellationToken cancellationToken)
    {
        var list = new List<GetPeopleResult> { new(), new(), new(), new() };

        var resultTotal = list.Count;

        var pagedList = list
            .Skip(request.PageNumber * request.PageSize)
            .Take(request.PageSize);

        return Task.FromResult(new GetPeopleQueryResult
        {
            Results = pagedList,
            Pagination = new QueryPagination
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalElements = resultTotal
            }
        });
    }
}