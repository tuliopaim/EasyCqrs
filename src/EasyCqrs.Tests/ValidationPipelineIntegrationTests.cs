using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;
using EasyCqrs.Tests.Config;
using System.Net;
using EasyCqrs.Commands;
using EasyCqrs.Sample.Application.Commands.Common;
using Xunit;

namespace EasyCqrs.Tests;

[Collection(nameof(IntegrationTestsFixture))]
public class ValidationPipelineIntegrationTests
{
    private readonly IntegrationTestsFixture _fixtures;

    public ValidationPipelineIntegrationTests(IntegrationTestsFixture fixtures)
    {
        _fixtures = fixtures;
    }

    [Fact]
    public async Task Must_Return_BadRequest_ErrorList_When_Invalid_Command()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var invalidPersonCommand = _fixtures.GetInvalidCommandInput();

        //act
        var (statusCode, result) = await _fixtures.Post<NewPersonCommandInput, CreatedCommandResult>(
            client, "/Person", invalidPersonCommand);

        //assert
        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result!.Errors);
    }

    [Fact]
    public async Task Must_Return_OK_When_Valid_Command()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var validPersonCommand = _fixtures.GetValidCommandInput();

        //act
        var (statusCode, result) = await _fixtures.Post<NewPersonCommandInput, CreatedCommandResult>(
            client, "/Person", validPersonCommand);

        //assert
        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(result);
        Assert.Empty(result!.Errors);
    }
        
    [Fact]
    public async Task Must_Return_BadRequest_ErrorList_When_Invalid_Query()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var getPeopleQueryInput = _fixtures.GetInvalidQueryInput();
        var queryParams = GetPeopleQueryPaginatedParams(getPeopleQueryInput);

        //act
        var (statusCode, result) = await _fixtures.Get<GetPeopleQueryPaginatedResult>(
            client, "/Person/paginated", queryParams);

        //assert
        Assert.Equal(HttpStatusCode.BadRequest, statusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result!.Errors);
    }

    [Fact]
    public async Task Must_Return_OK_When_Valid_Query()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var getPeopleQueryInput = _fixtures.GetValidQueryInput();
        var queryParams = GetPeopleQueryPaginatedParams(getPeopleQueryInput);

        //act
        var (statusCode, result) = await _fixtures.Get<GetPeopleQueryPaginatedResult>(
            client, "/Person/paginated", queryParams);

        //assert
        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(result);
        Assert.Empty(result!.Errors);
    }

    public Dictionary<string, string?> GetPeopleQueryPaginatedParams(GetPeopleQueryPaginatedInput query)
    {
        return new Dictionary<string, string?>
        {
            { nameof(query.Name), query.Name },
            { nameof(query.PageNumber), query.PageNumber.ToString() },
            { nameof(query.PageSize), query.PageSize.ToString() },
            { nameof(query.Age), query.Age.ToString() }
        };
    }
}
