using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Net;
using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;
using EasyCqrs.Tests.Models;
using Microsoft.VisualStudio.TestPlatform.TestHost;
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
        var invalidPersonCommand = new NewPersonCommandInput("Túlio Paim", "tulio@email.com", 0);
        return invalidPersonCommand;
    }

    public NewPersonCommandInput GetValidCommandInput()
    {
        var validPersonCommand = new NewPersonCommandInput("Túlio Paim", "tulio@email.com", 24);
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

    public async Task<(HttpStatusCode StatusCode, ApiResponse<TCommandResult?>? Result)> Post<TCommand, TCommandResult>(
        HttpClient httpClient,
        string endpoint,
        TCommand command) where TCommandResult : class
    {
        var json = JsonConvert.SerializeObject(command);

        var response = await httpClient.PostAsync(
            endpoint,
            new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            return (HttpStatusCode.InternalServerError, null);
        }

        var result = JsonConvert.DeserializeObject<ApiResponse<TCommandResult?>>(
            await response.Content.ReadAsStringAsync()) ?? throw new InvalidOperationException();

        return (response.StatusCode, result);
    }

    public async Task<(HttpStatusCode StatusCode, ApiResponse<TItem?>? Result)> Get<TItem>(
        HttpClient httpClient,
        string endpoint,
        Dictionary<string, string?> queryParams) where TItem : class
    {
        {
            var uri = QueryHelpers.AddQueryString(endpoint, queryParams);

            var response = await httpClient.GetAsync(uri);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return (HttpStatusCode.InternalServerError, null);
            }

            var result = JsonConvert.DeserializeObject<ApiResponse<TItem?>>(
                await response.Content.ReadAsStringAsync()) ?? throw new InvalidOperationException();

            return (response.StatusCode, result);
        }
    }
}


[CollectionDefinition(nameof(IntegrationTestsFixture))]
public class IntegrationTestsCollection : ICollectionFixture<IntegrationTestsFixture>
{
}