using EasyCqrs.Queries;
using EasyCqrs.Sample.Domain;

namespace EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;

public class GetPeoplePaginatedQueryHandler : IQueryHandler<GetPeoplePaginatedQueryInput, GetPeoplePaginatedQueryResult>
{
    public Task<GetPeoplePaginatedQueryResult> Handle(GetPeoplePaginatedQueryInput request, CancellationToken cancellationToken)
    {
        var filteredData = GetPersons();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filteredData = filteredData.Where(x => x.Name.Contains(request.Name));
        }

        if (request.Age != default)
        {
            filteredData = filteredData.Where(x => x.Age == request.Age);
        }

        // retreive your total filtered data count from your data source...
        var total = filteredData.Count();

        // retreive your paginated data from your data source...
        var paginatedResult = filteredData
            .OrderBy(x => x.Name)
            .Skip(request.PageNumber * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetPeopleResult
            {
                Id = x.Id,
                Name = x.Name,
                Age = x.Age,
            }).ToList();

        return Task.FromResult(new GetPeoplePaginatedQueryResult
        {
            Result = paginatedResult,
            Pagination = new QueryPagination
            {
                PageNumber = request.PageNumber,
                PageSize = paginatedResult.Count,
                TotalElements = total
            }
        });
    }

    private static IQueryable<Person> GetPersons()
    {
        var list = new List<Person>(20);

        for (var i = 1; i <= 20; i++)
        {
            list.Add(new Person($"Person {i:D2}", new Random().Next(20, 90)));
        }

        return list.AsQueryable();
    }
}