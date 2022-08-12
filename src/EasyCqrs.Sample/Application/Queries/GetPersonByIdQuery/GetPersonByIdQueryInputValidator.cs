using EasyCqrs.Queries;
using FluentValidation;

namespace EasyCqrs.Sample.Application.Queries.GetPersonByIdQuery;

public class GetPersonByIdQueryInputValidator : QueryInputValidator<GetPersonByIdQueryInput>
{
    public GetPersonByIdQueryInputValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}