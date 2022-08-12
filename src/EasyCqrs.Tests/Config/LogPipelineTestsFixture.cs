using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EasyCqrs.Tests.Config;
public class LogPipelineTestsFixture : IntegrationTestsFixture
{
    public WebApplicationFactory<Program> GetSampleApplicationWithServices(Action<IServiceCollection> configureServices)
    {
        var factory = base.GetSampleApplication()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(configureServices);
            });

        return factory;
    }
}

[CollectionDefinition(nameof(LogPipelineTestsFixture))]
public class LogPipelineFixturesCollection : ICollectionFixture<LogPipelineTestsFixture>
{
}
