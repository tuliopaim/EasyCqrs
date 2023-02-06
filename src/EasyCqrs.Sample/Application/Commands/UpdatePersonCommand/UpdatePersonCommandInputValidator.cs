using EasyCqrs.Commands;
using FluentValidation;

namespace EasyCqrs.Sample.Application.Commands.UpdatePersonCommand;

public class UpdatePersonCommandInputValidator : CommandInputValidator<UpdatePersonCommand>
{
    public UpdatePersonCommandInputValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150);

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18);
    }
}

