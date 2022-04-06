using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Sample.Application.Queries.GetPeoplePaginatedQuery;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace EasyCqrs.Tests.Config;

public class IntegrationTestsFixture
{
    public WebApplicationFactory<Program> GetSampleApplication()
    {
        return new WebApplicationFactory<Program>();
    }
    
    public NewPersonCommandInput GetValidCommandInput()
    {
        var invalidPersonCommand = new NewPersonCommandInput("Túlio Paim", 0);
        return invalidPersonCommand;
    }
    
    public NewPersonCommandInput GetInvalidCommandInput()
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
}   


[CollectionDefinition(nameof(IntegrationTestsFixture))]
public class IntegrationTestsCollection : ICollectionFixture<IntegrationTestsFixture>
{
}