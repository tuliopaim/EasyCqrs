using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Sample.Application.Queries.GetPeopleQueryPaginated;
using EasyCqrs.Tests.Config;
using FluentAssertions;
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
        var invalidPersonCommand = ValidationPipelineTestsFixture.GetInvalidCommandInput();

        //act
        var result = await _fixtures.Post<NewPersonCommand, Guid>(
            client, "/Person", invalidPersonCommand);

        //assert
        result.IsT0.Should().BeFalse();
        result.IsT1.Should().BeTrue();
        result.AsT1.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Must_NotReturn_BadRequest_ErrorList_When_Valid_Command()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var validPersonCommand = ValidationPipelineTestsFixture.GetValidCommandInput();

        //act
        var result = await _fixtures.Post<NewPersonCommand, Guid>(
            client, "/Person", validPersonCommand);

        //assert
        result.IsT0.Should().BeTrue();
        result.IsT1.Should().BeFalse();
        result.AsT0.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Must_Return_OK_When_Valid_Query()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var queryInput = ValidationPipelineTestsFixture.GetValidQueryInput();

        //act
        var result = await _fixtures.Get<GetPeopleQueryPaginated, PaginatedList<GetPeopleQueryPaginatedItem>>(
            client, "/Person/paginated", queryInput);

        //assert
        result.IsT0.Should().BeTrue();
        result.IsT1.Should().BeFalse();
        result.AsT0.Result.Should().NotBeNull();
        result.AsT0.Result!.Pagina.Should().Be(queryInput.PageNumber);
    }
}
