# Integration Tests Framework

A framework for writing integration tests with built-in support for JWT authentication, in-memory databases, and DynamoDB mocking.

## Quick Start

Create a test class that inherits from `IntegrationTestBase`:

```csharp
public class MyTest : IntegrationTestBase, IAsyncLifetime
{
    public Task InitializeAsync()
    {
        // Seed test data
        return SeedTestDbSetAsync(new MyEntity { Id = Guid.NewGuid() });
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task TestGet()
    {
        var response = await TestClient
            .WithJwtBearerM2MTokenIncludingScopes(scopes: "scope.read")
            .GetAsync("/api/v1/resource/123");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

## Features

### JWT Authentication

Add JWT bearer tokens with scopes to test requests:

```csharp
TestClient
    .WithJwtBearerM2MTokenIncludingScopes(scopes: "scope.write", "scope.read")
    .PostAsync("/api/resource", content);
```

Remove authentication:

```csharp
TestClient.WithoutAuthHeader();
```

### Database Testing

Seed test data:

```csharp
await SeedTestDbSetAsync(entity1, entity2);
```

Verify database state:

```csharp
await WithDbContextAsync(async dbContext =>
{
    var item = await dbContext.MyEntities.FindAsync(id);
    item.Should().NotBeNull();
});
```

### DynamoDB Mocking

Access the mocked DynamoDB client:

```csharp
DynamoDBTestClient.Setup(x => x.GetItemAsync(...))
    .ReturnsAsync(...);
```

## Configuration

Override test configuration by overriding `GetTestConfiguration()`:

```csharp
protected override Dictionary<string, string?> GetTestConfiguration()
{
    return new Dictionary<string, string?>
    {
        ["Custom:Setting"] = "value"
    };
}
```

