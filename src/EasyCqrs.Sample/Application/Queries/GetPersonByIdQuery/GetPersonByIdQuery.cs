using FluentValidation;

namespace EasyCqrs.Sample.Application.Queries.GetPersonByIdQuery;

public record GetPersonByIdQuery(Guid Id) : IQuery<GetPersonByIdQueryItem>
{
}

public class GetPersonByIdQueryInputValidator : AbstractValidator<GetPersonByIdQuery>
{
    public GetPersonByIdQueryInputValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
