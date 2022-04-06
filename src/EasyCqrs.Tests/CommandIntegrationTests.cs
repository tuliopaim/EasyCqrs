using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Tests.Config;
using Newtonsoft.Json;
using Xunit;

namespace EasyCqrs.Tests;

[Collection(nameof(IntegrationTestsFixture))]
public class CommandIntegrationTests
{
    private readonly IntegrationTestsFixture _fixtures;

    public CommandIntegrationTests(IntegrationTestsFixture fixtures)
    {
        _fixtures = fixtures;
    }

    [Fact]
    public async Task Should_Return_ErrorList_When_Invalid()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var invalidPersonCommand = _fixtures.GetValidCommandInput();

        //act
        var newPersonCommandResult = await PostCommand<NewPersonCommandInput, NewPersonCommandResult>(client, invalidPersonCommand);

        //assert
        Assert.NotNull(newPersonCommandResult);
        Assert.NotEmpty(newPersonCommandResult!.Errors);
        Assert.Equal(Guid.Empty, newPersonCommandResult.Id);
    }


    [Fact]
    public async Task ShouldNot_Return_ErrorList_When_Valid()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var validPersonCommand = _fixtures.GetInvalidCommandInput();

        //act
        var newPersonCommandResult = await PostCommand<NewPersonCommandInput, NewPersonCommandResult>(client, validPersonCommand);

        //assert
        Assert.NotNull(newPersonCommandResult);
        Assert.Empty(newPersonCommandResult!.Errors);
        Assert.NotEqual(Guid.Empty, newPersonCommandResult.Id);
    }

    private static async Task<TCommandResult> PostCommand<TCommand, TCommandResult>(HttpClient httpClient, TCommand command)
    {
        var json = JsonConvert.SerializeObject(command);

        var response = await httpClient.PostAsync(
            "/Person",
            new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

        return JsonConvert.DeserializeObject<TCommandResult>(
            await response.Content.ReadAsStringAsync()) ?? throw new InvalidOperationException();
    }
    
}