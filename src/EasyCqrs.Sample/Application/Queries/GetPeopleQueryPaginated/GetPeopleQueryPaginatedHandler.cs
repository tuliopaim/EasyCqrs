using EasyCqrs.Results;
using EasyCqrs.Sample.Domain;
using EasyCqrs.Sample.Repositories;

namespace EasyCqrs.Sample.Application.Queries.GetPeopleQueryPaginated;

public class GetPeopleQueryPaginatedHandler : IQueryHandler<GetPeopleQueryPaginated, PaginatedList<GetPeopleQueryPaginatedItem>>
{
    private readonly IPersonRepository _repository;

    public GetPeopleQueryPaginatedHandler(IPersonRepository repository)
    {
        _repository = repository;
    }

    public Task<Result<PaginatedList<GetPeopleQueryPaginatedItem>>> Handle(
        GetPeopleQueryPaginated request,
        CancellationToken cancellationToken)
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

        var paginatedList = new PaginatedList<GetPeopleQueryPaginatedItem>(
            paginatedResult, 
            total, 
            request.PageNumber,
            request.PageSize);
        
        return Task.FromResult(Result.Success(paginatedList));
    }

    private IQueryable<Person> GetFilteredPeople(GetPeopleQueryPaginated request)
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