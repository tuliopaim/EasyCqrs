using EasyCqrs.Commands;
using EasyCqrs.Sample.Application.Commands.NotificationCommand;
using EasyCqrs.Tests.Config;
using System.Net;
using EasyCqrs.Sample.Application.Commands.Common;
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
        const string notificationMessage = "New notification!";
        var notificationInput = new NotificationCommandInput(notificationMessage);

        //act
        var (statusCode, result) = await _fixtures.Post<NotificationCommandInput, CommandResult>(
            client, "/Notification", notificationInput);

        //assert
        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result!.Errors);
        Assert.Equal(notificationMessage, result!.Errors.First());
    }
}
