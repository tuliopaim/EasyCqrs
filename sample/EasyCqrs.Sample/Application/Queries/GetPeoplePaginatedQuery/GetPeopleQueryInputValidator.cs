using EasyCqrs.Queries;
using FluentValidation;

namespace EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;

public class GetPeopleQueryInputValidator : QueryInputValidator<GetPeopleQueryInput>
{
    public GetPeopleQueryInputValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}