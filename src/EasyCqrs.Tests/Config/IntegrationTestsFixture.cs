using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Net;
using EasyCqrs.Tests.Models;
using Xunit;

namespace EasyCqrs.Tests.Config;

public class IntegrationTestsFixture
{
    public virtual WebApplicationFactory<Program> GetSampleApplication()
    {
        return new WebApplicationFactory<Program>();
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

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<ApiResponse<TCommandResult?>>(content)
            ?? throw new InvalidOperationException();

        return (response.StatusCode, result);
    }

    public Task<(HttpStatusCode StatusCode, ApiResponse<TItem?>? Result)> Get<TItem>(
        HttpClient httpClient,
        string endpoint,
        Dictionary<string, string?> queryParams) where TItem : class
    {
        return GetBase<ApiResponse<TItem?>>(httpClient, endpoint, queryParams);
    }

    public Task<(HttpStatusCode StatusCode, ApiListResponse<TItem?>? Result)> GetList<TItem>(
        HttpClient httpClient,
        string endpoint,
        Dictionary<string, string?> queryParams) where TItem : class
    {
        return GetBase<ApiListResponse<TItem?>>(httpClient, endpoint, queryParams);
    }

    public Task<(HttpStatusCode StatusCode, ApiPaginatedResponse<TItem?>? Result)> GetPaginated<TItem>(
        HttpClient httpClient,
        string endpoint,
        Dictionary<string, string?> queryParams) where TItem : class
    {
        return GetBase<ApiPaginatedResponse<TItem?>>(httpClient, endpoint, queryParams);
    }

    private async Task<(HttpStatusCode StatusCode, TResult? Result)> GetBase<TResult>(
        HttpClient httpClient,
        string endpoint,
        Dictionary<string, string?> queryParams) where TResult : class
    {
        var uri = QueryHelpers.AddQueryString(endpoint, queryParams);

        var response = await httpClient.GetAsync(uri);

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            return (HttpStatusCode.InternalServerError, null);
        }

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<TResult>(content)
            ?? throw new InvalidOperationException();

        return (response.StatusCode, result);
    }

}

[CollectionDefinition(nameof(IntegrationTestsFixture))]
public class IntegrationTestsCollection : ICollectionFixture<IntegrationTestsFixture>
{
}