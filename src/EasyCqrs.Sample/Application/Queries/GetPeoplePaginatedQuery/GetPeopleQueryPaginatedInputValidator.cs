using EasyCqrs.Queries;
using FluentValidation;

namespace EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;

public class GetPeopleQueryPaginatedInputValidator : QueryInputValidator<GetPeopleQueryPaginatedInput>
{
    public GetPeopleQueryPaginatedInputValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}