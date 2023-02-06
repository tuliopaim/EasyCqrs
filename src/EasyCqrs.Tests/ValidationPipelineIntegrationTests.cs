using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Sample.Application.Queries.GetPeopleQueryPaginated;
using EasyCqrs.Tests.Config;
using InteligenteZap.Domain.Shared;
using System.Net;
using Xunit;

namespace EasyCqrs.Tests;

[Collection(nameof(ValidationPipelineTestsFixture))]
public class ValidationPipelineIntegrationTests
{
    private readonly ValidationPipelineTestsFixture _fixtures;

    public ValidationPipelineIntegrationTests(ValidationPipelineTestsFixture fixtures)
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
        var (result, error) = await _fixtures.Post<NewPersonCommand, Guid>(
            client, "/Person", invalidPersonCommand);

        //assert
        Assert.Equal(default, result);
        Assert.NotNull(error);
        Assert.NotNull(error?.Message);
    }

    [Fact]
    public async Task Must_NotReturn_BadRequest_ErrorList_When_Valid_Command()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var validPersonCommand = _fixtures.GetValidCommandInput();

        //act
        var (result, error) = await _fixtures.Post<NewPersonCommand, Guid>(
            client, "/Person", validPersonCommand);

        //assert
        Assert.NotEqual(default, result);
        Assert.Null(error);
    }

    [Fact]
    public async Task Must_Return_OK_When_Valid_Query()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var getPeopleQueryInput = _fixtures.GetValidQueryInput();
        var queryParams = GetPeopleQueryPaginatedParams(getPeopleQueryInput);

        //act
        var (statusCode, result) = await _fixtures.Get<PaginatedList<GetPeopleQueryPaginatedItem>>(
            client, "/Person/paginated", queryParams);

        //assert
        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(result);
    }

    private Dictionary<string, string?> GetPeopleQueryPaginatedParams(GetPeopleQueryPaginated query)
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
