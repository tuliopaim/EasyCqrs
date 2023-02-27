using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using OneOf;
using System.Net;
using Xunit;

namespace EasyCqrs.Tests.Config;

public class IntegrationTestsFixture
{
    public virtual WebApplicationFactory<Program> GetSampleApplication()
    {
        return new WebApplicationFactory<Program>();
    }

    public async Task<OneOf<TCommandResult, ErrorDto>> Post<TCommand, TCommandResult>(
        HttpClient httpClient,
        string endpoint,
        TCommand command) 
    {
        var json = JsonConvert.SerializeObject(command);

        var response = await httpClient.PostAsync(
            endpoint,
            new StringContent(json, System.Text.Encoding.UTF8, "application/json"));

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            return new ErrorDto(new List<string> { "Internal Error" });
        }

        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<TCommandResult>(content)
                ?? throw new InvalidOperationException();

            return result;
        }

        var errors = JsonConvert.DeserializeObject<ErrorDto>(content)
            ?? throw new InvalidOperationException();

        return errors;
    }

    public async Task<OneOf<(HttpStatusCode StatusCode, TResult? Result), ErrorDto>> Get<TQuery, TResult>(
        HttpClient httpClient,
        string endpoint,
        TQuery query)
    {
        var queryParams = QueryParams(query);
        var uri = QueryHelpers.AddQueryString(endpoint, queryParams!);

        var response = await httpClient.GetAsync(uri);

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            return (HttpStatusCode.InternalServerError, default);
        }

        var content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return (HttpStatusCode.NotFound, default);
        }

        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<TResult>(content)
                ?? throw new InvalidOperationException();

            return (response.StatusCode, result);
        }

        var errors = JsonConvert.DeserializeObject<ErrorDto>(content)
            ?? throw new InvalidOperationException();

        return errors;
    }
    

    private static Dictionary<string, string> QueryParams<TQuery>(TQuery query)
    {
        var querySerialized = JsonConvert.SerializeObject(query);

        var queryAsDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(querySerialized);
        
        return queryAsDictionary!;
    }
}

[CollectionDefinition(nameof(IntegrationTestsFixture))]
public class IntegrationTestsCollection : ICollectionFixture<IntegrationTestsFixture>
{
}

public record ErrorDto(List<string> Errors);