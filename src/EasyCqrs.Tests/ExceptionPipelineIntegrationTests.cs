using EasyCqrs.Commands;
using EasyCqrs.Queries;
using EasyCqrs.Sample.Application.Commands.ExceptionThrownCommand;
using EasyCqrs.Tests.Config;
using System.Net;
using Xunit;

namespace EasyCqrs.Tests;

[Collection(nameof(IntegrationTestsFixture))]
public class ExceptionPipelineIntegrationTests
{
    private readonly IntegrationTestsFixture _fixtures;

    public ExceptionPipelineIntegrationTests(IntegrationTestsFixture fixtures)
    {
        _fixtures = fixtures;
    }
    
    [Fact]
    public async void Must_Return_BadRequest_ErrorList_When_ExceptionThrown_Command()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var input = new ExceptionThrownCommandInput();

        //act
        (var statusCode, var result) = await _fixtures.Post<ExceptionThrownCommandInput, CommandResult>
            (client, "/ExceptionThrown", input);

        //assert
        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result!.Errors);
    }

    [Fact]
    public async void Must_Return_BadRequest_ErrorList_When_ExceptionThrown_Query()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();

        //act
        (var statusCode, var result) = await _fixtures.Get<QueryResult<int>>(client, "/ExceptionThrown", new());

        //assert
        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result!.Errors);
    }
}   