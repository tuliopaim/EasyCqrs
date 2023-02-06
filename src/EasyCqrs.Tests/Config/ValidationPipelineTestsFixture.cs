using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Sample.Application.Queries.GetPeopleQueryPaginated;
using Xunit;

namespace EasyCqrs.Tests.Config;

public class ValidationPipelineTestsFixture : IntegrationTestsFixture
{
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

    public GetPeopleQueryPaginated GetInvalidQueryInput()
    {
        var invalidPersonCommand = new GetPeopleQueryPaginated
        {
            PageNumber = -1,
            PageSize = -1
        };

        return invalidPersonCommand;
    }

    public GetPeopleQueryPaginated GetValidQueryInput()
    {
        var invalidPersonCommand = new GetPeopleQueryPaginated
        {
            PageNumber = 2,
            PageSize = 50
        };

        return invalidPersonCommand;
    }
}

[CollectionDefinition(nameof(ValidationPipelineTestsFixture))]
public class ValidationPipelineFixturesCollection : ICollectionFixture<ValidationPipelineTestsFixture>
{
}
