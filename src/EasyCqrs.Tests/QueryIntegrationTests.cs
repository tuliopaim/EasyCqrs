using EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;
using EasyCqrs.Tests.Config;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Xunit;

namespace EasyCqrs.Tests;

[Collection(nameof(IntegrationTestsFixture))]
public class QueryIntegrationTests
{
    private readonly IntegrationTestsFixture _fixtures;

    public QueryIntegrationTests(IntegrationTestsFixture fixtures)
    {
        _fixtures = fixtures;
    }
    
    [Fact]
    public async Task Should_Return_ErrorList_When_Invalid()
    {
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var getPeopleQueryInput = _fixtures.GetInvalidQueryInput();

        //act
        var getPeopleQueryResult = await GetPeopleQuery(client, getPeopleQueryInput);

        //assert
        Assert.NotNull(getPeopleQueryResult);
        Assert.NotEmpty(getPeopleQueryResult!.Errors);
    }

    [Fact]
    public async Task ShouldNot_Return_ErrorList_When_Valid()
    {   
        //arrange
        var client = _fixtures.GetSampleApplication().CreateClient();
        var getPeopleQueryInput = _fixtures.GetValidQueryInput();

        //act
        var getPeopleQueryResult = await GetPeopleQuery(client, getPeopleQueryInput);

        //assert
        Assert.NotNull(getPeopleQueryResult);
        Assert.Empty(getPeopleQueryResult!.Errors);
    }

    private static async Task<GetPeopleQueryPaginatedResult> GetPeopleQuery(HttpClient httpClient, GetPeopleQueryPaginatedInput query)
    {
        var json = JsonConvert.SerializeObject(query);

        var queryStringDict = new Dictionary<string, string?>
        {
            { nameof(query.Name), query.Name },
            { nameof(query.PageNumber), query.PageNumber.ToString() },
            { nameof(query.PageSize), query.PageSize.ToString() },
            { nameof(query.Age), query.Age.ToString() }
        };

        var uri = QueryHelpers.AddQueryString("/Person/paginated", queryStringDict);

        var response = await httpClient.GetAsync(uri);
        
        return JsonConvert.DeserializeObject<GetPeopleQueryPaginatedResult>(
            await response.Content.ReadAsStringAsync()) ?? throw new InvalidOperationException();
    }

}