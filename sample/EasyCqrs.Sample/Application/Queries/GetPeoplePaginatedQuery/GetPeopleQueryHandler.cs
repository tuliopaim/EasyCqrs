using EasyCqrs.Queries;
using EasyCqrs.Sample.Domain;

namespace EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;

public class GetPeopleQueryHandler : IQueryHandler<GetPeopleQueryInput, GetPeopleQueryResult>
{
    public Task<GetPeopleQueryResult> Handle(GetPeopleQueryInput request, CancellationToken cancellationToken)
    {
        var list = new List<Person>(20);

        for (var i = 1; i <= 20; i++)
        {
            list.Add(new Person($"Person {i:D2}", new Random().Next(20, 90)));
        }

        return Task.FromResult(new GetPeopleQueryResult
        {
            Result = list
                .OrderBy(x => x.Name)
                .Skip(request.PageNumber * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new GetPeopleResult
                {
                    Id = x.Id,
                    Name = x.Name,
                    Age = x.Age,
                }),
            Pagination = new QueryPagination
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalElements = list.Count
            }
        });

    }
}