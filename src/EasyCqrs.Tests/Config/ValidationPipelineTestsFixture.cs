using EasyCqrs.Sample.Application.Commands.NewPersonCommand;
using EasyCqrs.Sample.Application.Queries.GetPeopleQueryPaginated;
using Xunit;

namespace EasyCqrs.Tests.Config;

public class ValidationPipelineTestsFixture : IntegrationTestsFixture
{
    public static NewPersonCommand GetInvalidCommandInput()
    {
        var invalidPersonCommand = new NewPersonCommand("Túlio Paim", "tulio@email.com", 0);
        return invalidPersonCommand;
    }

    public static NewPersonCommand GetValidCommandInput()
    {
        var validPersonCommand = new NewPersonCommand("Túlio Paim", "tulio@email.com", 24);
        return validPersonCommand;
    }

    public static GetPeopleQueryPaginated GetInvalidQueryInput()
    {
        var invalidPersonCommand = new GetPeopleQueryPaginated
        {
            PageNumber = -1,
            PageSize = -1
        };

        return invalidPersonCommand;
    }

    public static GetPeopleQueryPaginated GetValidQueryInput()
    {
        var invalidPersonCommand = new GetPeopleQueryPaginated
        {
            PageNumber = 2,
            PageSize = 2
        };

        return invalidPersonCommand;
    }
}

[CollectionDefinition(nameof(ValidationPipelineTestsFixture))]
public class ValidationPipelineFixturesCollection : ICollectionFixture<ValidationPipelineTestsFixture>
{
}
