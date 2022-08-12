using EasyCqrs.Queries;
using EasyCqrs.Sample.Domain;
using EasyCqrs.Sample.Repositories;

namespace EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;

public class GetPeopleQueryPaginatedHandler : IQueryHandler<GetPeopleQueryPaginatedInput, GetPeopleQueryPaginatedResult>
{
    private readonly IPersonRepository _repository;

    public GetPeopleQueryPaginatedHandler(IPersonRepository repository)
    {
        _repository = repository;
    }

    public Task<GetPeopleQueryPaginatedResult> Handle(GetPeopleQueryPaginatedInput request, CancellationToken cancellationToken)
    {
        var filteredData = GetFilteredPeople(request);
        
        var total = filteredData.Count();
        
        var paginatedResult = filteredData
            .OrderBy(x => x.Name)
            .Skip(request.PageNumber * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetPeopleQueryPaginatedItem
            {
                Id = x.Id,
                Name = x.Name,
                Age = x.Age,
            }).ToList();

        return Task.FromResult(new GetPeopleQueryPaginatedResult
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

    private IQueryable<Person> GetFilteredPeople(GetPeopleQueryPaginatedInput request)
    {
        var filteredData = _repository.GetPeople();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filteredData = filteredData.Where(x => x.Name.Contains(request.Name));
        }

        if (request.Age != default)
        {
            filteredData = filteredData.Where(x => x.Age == request.Age);
        }

        return filteredData;
    }
}