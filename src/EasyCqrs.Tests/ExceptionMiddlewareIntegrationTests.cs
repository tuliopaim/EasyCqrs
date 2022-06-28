using EasyCqrs.Commands;
using EasyCqrs.Queries;
using EasyCqrs.Sample.Application.Commands.DivideByZeroCommand;
using EasyCqrs.Tests.Config;
using System.Net;
using Xunit;

namespace EasyCqrs.Tests;

[Collection(nameof(IntegrationTestsFixture))]
public class ExceptionMiddlewareIntegrationTests
{
    private readonly IntegrationTestsFixture _fixtures;

    public ExceptionMiddlewareIntegrationTests(IntegrationTestsFixture fixtures)
    {
        _fixtures = fixtures;
    }
    
    [Fact]
    public async void Must_Return_Status500_When_DivideByZero_Command()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var input = new DivideByZeroCommandInput();

        //act
        var (statusCode, _) = await _fixtures.Post<DivideByZeroCommandInput, CommandResult>
            (client, "/DivideByZero", input);

        //assert
        Assert.Equal(HttpStatusCode.InternalServerError, statusCode);
    }

    [Fact]
    public async void Must_Return_Status500_When_DivideByZero_Query()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();

        //act
        var (statusCode, _) = await _fixtures.Get<QueryResult<int>>(client, "/DivideByZero", new Dictionary<string, string?>());

        //assert
        Assert.Equal(HttpStatusCode.InternalServerError, statusCode);
    }
}   