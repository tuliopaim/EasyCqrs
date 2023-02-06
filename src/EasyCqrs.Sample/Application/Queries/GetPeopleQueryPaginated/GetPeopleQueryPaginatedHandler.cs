using EasyCqrs.Results;
using EasyCqrs.Sample.Domain;
using EasyCqrs.Sample.Repositories;

namespace EasyCqrs.Sample.Application.Queries.GetPeopleQueryPaginated;

public class GetPeopleQueryPaginatedHandler : IQueryHandler<GetPeopleQueryPaginated, Pagination<GetPeopleQueryPaginatedItem>>
{
    private readonly IPersonRepository _repository;

    public GetPeopleQueryPaginatedHandler(IPersonRepository repository)
    {
        _repository = repository;
    }

    public Task<Result<Pagination<GetPeopleQueryPaginatedItem>>> Handle(
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

        var result = Result.Success(new Pagination<GetPeopleQueryPaginatedItem>
        { 
            Itens = paginatedResult,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalElements = total,
        });

        return Task.FromResult(result);
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