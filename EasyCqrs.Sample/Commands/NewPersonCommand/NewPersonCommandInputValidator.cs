using EasyCqrs.Commands;
using FluentValidation;

namespace EasyCqrs.Sample.Commands.NewPersonCommand;

public class NewPersonCommandInputValidator : CommandInputValidator<NewPersonCommandInput>
{
    public NewPersonCommandInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(150);

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18);
    }
}