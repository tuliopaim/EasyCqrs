using EasyCqrs.Commands;
using EasyCqrs.Pipelines;
using EasyCqrs.Sample.Application.Commands.NotificationCommand;
using EasyCqrs.Tests.Config;
using EasyCqrs.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace EasyCqrs.Tests;


[Collection(nameof(LogPipelineTestsFixture))]
public class LogPipelineIntegrationTests
{
    private readonly LogPipelineTestsFixture _fixtures;

    public LogPipelineIntegrationTests(LogPipelineTestsFixture fixtures)
    {
        _fixtures = fixtures;
    }

    [Fact]
    public async Task Must_Invoke_LogBeforeAsync()
    {
        //arrange
        var pipelineLogServiceMock = new Mock<IPipelineLogService>();

        var client = _fixtures.GetSampleApplicationWithServices(services =>
        {
            services.AddScoped(_ => pipelineLogServiceMock.Object);
        }).CreateClient();

        var notification = new NotificationCommandInput("Notification");

        //act
        var (statusCode, result)  = await _fixtures.Post<NotificationCommandInput, ApiResponse<CommandResult>>(
            client,
            "Notification",
            notification);

        //assert
        pipelineLogServiceMock.Verify(x => x.LogBeforeAsync(notification, default), Times.Once);
    }

    [Fact]
    public async Task Must_Invoke_LogAfterAsync()
    {
        //arrange
        var pipelineLogServiceMock = new Mock<IPipelineLogService>();

        var client = _fixtures.GetSampleApplicationWithServices(services =>
        {
            services.AddScoped(_ => pipelineLogServiceMock.Object);
        }).CreateClient();

        var notification = new NotificationCommandInput("Notification");

        //act
        var (statusCode, result)  = await _fixtures.Post<NotificationCommandInput, ApiResponse<CommandResult>>(
            client,
            "Notification",
            notification);

        //assert
        pipelineLogServiceMock.Verify(x => x.LogAfterAsync(notification, result, default), Times.Once);
    }
}
