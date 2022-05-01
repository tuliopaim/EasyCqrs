using EasyCqrs.Commands;
using FluentValidation;

namespace EasyCqrs.Sample.Application.Commands.NotificationCommand;

public class NotificationCommandInputValidator : CommandInputValidator<NotificationCommandInput>
{
    public NotificationCommandInputValidator()
    {
        RuleFor(x => x.Notification).NotEmpty();
    }
}
