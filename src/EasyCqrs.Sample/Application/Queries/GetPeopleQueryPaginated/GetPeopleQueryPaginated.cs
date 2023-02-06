using FluentValidation;

namespace EasyCqrs.Sample.Application.Queries.GetPeopleQueryPaginated;

public class GetPeopleQueryPaginated : QueryPaginated<GetPeopleQueryPaginatedItem>
{
    public string? Name { get; set; }
    public int? Age { get; set; }
}

public class GetPeopleQueryPaginatedInputValidator : AbstractValidator<GetPeopleQueryPaginated>
{
    public GetPeopleQueryPaginatedInputValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}