using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace EasyCqrs.Tests.Config;

public class IntegrationTestsFixture
{
    public virtual WebApplicationFactory<Program> GetSampleApplication()
    {
        return new WebApplicationFactory<Program>();
    }

    public async Task<(TCommandResult? Result, ErrorDto? Error)> Post<TCommand, TCommandResult>(
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
            return (default, null);
        }

        var content = await response.Content.ReadAsStringAsync();

        TCommandResult? result = default;
        ErrorDto? error = null;

        if (response.IsSuccessStatusCode)
        {
            result = JsonConvert.DeserializeObject<TCommandResult>(content)
                ?? throw new InvalidOperationException();
        }
        else
        {
            error = JsonConvert.DeserializeObject<ErrorDto>(content)
                ?? throw new InvalidOperationException();
        }

        return (result, error);
    }

    public async Task<(HttpStatusCode StatusCode, TResult? Result)> Get<TResult>(
        HttpClient httpClient,
        string endpoint,
        Dictionary<string, string?> queryParams)
    {
        var uri = QueryHelpers.AddQueryString(endpoint, queryParams);

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

        var result = JsonConvert.DeserializeObject<TResult>(content)
            ?? throw new InvalidOperationException();

        return (response.StatusCode, result);
    }
}

[CollectionDefinition(nameof(IntegrationTestsFixture))]
public class IntegrationTestsCollection : ICollectionFixture<IntegrationTestsFixture>
{
}

public record ErrorDto(string Message);