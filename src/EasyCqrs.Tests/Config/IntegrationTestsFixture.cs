using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace EasyCqrs.Tests.Config;

public class IntegrationTestsFixture
{
    public WebApplicationFactory<Program> GetSampleApplication()
    {
        return new WebApplicationFactory<Program>();
    }
    
    public NewPersonCommandInput GetInvalidCommandInput()
    {
        var invalidPersonCommand = new NewPersonCommandInput("Túlio Paim", 0);
        return invalidPersonCommand;
    }
    
    public NewPersonCommandInput GetValidCommandInput()
    {
        var validPersonCommand = new NewPersonCommandInput("Túlio Paim", 24);
        return validPersonCommand;
    }
    
    public GetPeopleQueryPaginatedInput GetInvalidQueryInput()
    {
        var invalidPersonCommand = new GetPeopleQueryPaginatedInput
        {
            PageNumber = -1,
            PageSize = -1
        };
        return invalidPersonCommand;
    }

    public GetPeopleQueryPaginatedInput GetValidQueryInput()
    {
        var invalidPersonCommand = new GetPeopleQueryPaginatedInput
        {
            PageNumber = 2,    
            PageSize = 50
        };
        return invalidPersonCommand;
    }

    public async Task<(HttpStatusCode StatudCode, TCommandResult Result)> Post<TCommand, TCommandResult>(
        HttpClient httpClient,
        string endpoint,
        TCommand command)
    {
        var json = JsonConvert.SerializeObject(command);

        var response = await httpClient.PostAsync(
            endpoint,
            new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
                
        var result = JsonConvert.DeserializeObject<TCommandResult>(
            await response.Content.ReadAsStringAsync()) ?? throw new InvalidOperationException();

        return (response.StatusCode, result);
    }

    public async Task<(HttpStatusCode StatudCode, TQueryResult Result)> Get<TQueryResult>(
        HttpClient httpClient,
        string endpoint,
        Dictionary<string, string?> queryParams)
    {
        var uri = QueryHelpers.AddQueryString(endpoint, queryParams);

        var response = await httpClient.GetAsync(uri);

        var result = JsonConvert.DeserializeObject<TQueryResult>(
            await response.Content.ReadAsStringAsync()) ?? throw new InvalidOperationException();
        
        return (response.StatusCode, result);
    }
}   


[CollectionDefinition(nameof(IntegrationTestsFixture))]
public class IntegrationTestsCollection : ICollectionFixture<IntegrationTestsFixture>
{
}