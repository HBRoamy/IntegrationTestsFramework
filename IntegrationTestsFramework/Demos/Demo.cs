using FluentAssertions;
using IntegrationTestsFramework.Framework;
using IntegrationTestsFramework.Placeholders;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTestsFramework.Demos;

public class Demo : IntegrationTestBase, IAsyncLifetime
{
    private readonly PlaceholderEntity _entityWithUserId;
    private readonly PlaceholderEntity _entityWithoutUserId;

    public Demo()
    {
        _entityWithUserId = new PlaceholderEntity(uuid: Guid.NewGuid(), userId: Guid.NewGuid());
        _entityWithoutUserId = new PlaceholderEntity(uuid: Guid.NewGuid(), userId: null);
    }

    public Task InitializeAsync()
    {
        return SeedTestDbSetAsync(_entityWithUserId, _entityWithoutUserId);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData("v1")]
    public async Task TestGet(string version)
    {
        var response = await TestClient
            .WithJwtBearerM2MTokenIncludingScopes(scopes: TestConstants.Scopes.ReadIdentityScope)
            .GetAsync(ApiEndpoint(version, Guid.NewGuid().ToString()));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("v1")]
    public async Task TestPost(string version)
    {
        var request = new PlaceholderEntity(uuid: Guid.NewGuid(), userId: Guid.NewGuid());

        var response = await TestClient
            .WithJwtBearerM2MTokenIncludingScopes(scopes: TestConstants.Scopes.WriteMappingScope)
            .PostAsync(ApiEndpoint(version, request.UUId.ToString()), JsonContent.Create(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Verify the entity was persisted in the database
        await WithDbContextAsync(async dbContext =>
        {
            var savedItem = await dbContext.PlaceholderEntities.FindAsync(request.UUId);

            savedItem.Should().NotBeNull();

            savedItem?.UserId.Should().Be(request.UserId);
        });

        // Verify response body
        var responseBody = await response.Content.ReadAsStringAsync();
        responseBody.Should().NotBeNull();
    }

    #region Private Methods

    private string ApiEndpoint(string version, string uuid) => 
        $"{TestClient.BaseAddress}api/{version}/resource/{uuid}";

    #endregion
}
