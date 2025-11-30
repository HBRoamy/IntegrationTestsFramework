using IntegrationTestsFramework.Placeholders;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IntegrationTestsFramework.Framework;

public class IntegrationTestBase : IDisposable
{
    protected readonly HttpClient TestClient;
    protected readonly Mock<PlaceholderIAmazonDynamoDB> DynamoDBTestClient;
    private readonly WebApplicationFactory<PlaceholderProgram> Factory;

    protected IntegrationTestBase()
    {
        DynamoDBTestClient = new Mock<PlaceholderIAmazonDynamoDB>();

        Factory = new WebApplicationFactory<PlaceholderProgram>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(GetTestConfiguration());
                });

                builder.ConfigureTestServices(services =>
                {
                    services.AddInMemoryTestDatabase<PlaceholderDbContext>();
                    services.AddTestJwtAuthentication();
                    services.AddTestDynamoDBClient(DynamoDBTestClient.Object);
                    services.AddTestAuthorizationPolicies();
                });
            });

        TestClient = Factory.CreateClient();
    }

    /// <summary>
    /// Executes an async function with a scoped ApplicationDbContext for database verification in tests.
    /// </summary>
    protected async Task WithDbContextAsync(Func<PlaceholderDbContext, Task> action)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PlaceholderDbContext>();
        await action(dbContext);
    }

    protected async Task SeedTestDbSetAsync<TEntity>(params TEntity[] entities) where TEntity : class
    {
        await WithDbContextAsync(async dbContext =>
        {
            dbContext.Set<TEntity>().AddRange(entities);
            await dbContext.SaveChangesAsync();
        });
    }

    protected virtual Dictionary<string, string?> GetTestConfiguration()
    {
        return new Dictionary<string, string?>
        {
            [TestConstants.DynamoDb.ConfigKeys.Region] = TestConstants.DynamoDb.ConfigValues.Region,
            [TestConstants.DynamoDb.ConfigKeys.RoleArn] = TestConstants.DynamoDb.ConfigValues.RoleArn,
            [TestConstants.DynamoDb.ConfigKeys.TableName] = TestConstants.DynamoDb.ConfigValues.TableName
        };
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            TestClient?.Dispose();
            Factory?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
