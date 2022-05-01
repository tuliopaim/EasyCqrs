using EasyCqrs.Commands;
using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Sample.Application.Commands.NotificationCommand;
using EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;
using EasyCqrs.Tests.Config;
using System.Net;
using Xunit;

namespace EasyCqrs.Tests;

[Collection(nameof(IntegrationTestsFixture))]
public class NotificationPipelineIntegrationTests
{
    private readonly IntegrationTestsFixture _fixtures;

    public NotificationPipelineIntegrationTests(IntegrationTestsFixture fixtures)
    {
        _fixtures = fixtures;
    }

    [Fact]
    public async Task Must_Return_BadRequest_ErrorList_When_Has_Notification_Command()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        const string NotificationMessage = "New notification!";
        var notificationInput = new NotificationCommandInput(NotificationMessage);

        //act
        (var statusCode, var result) = await _fixtures.Post<NotificationCommandInput, CommandResult>(
            client, "/Notification", notificationInput);

        //assert
        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result!.Errors);
        Assert.Equal(NotificationMessage, result!.Errors.First());
    }
}
