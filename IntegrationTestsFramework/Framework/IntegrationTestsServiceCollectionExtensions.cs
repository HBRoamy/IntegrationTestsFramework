using IntegrationTestsFramework.Placeholders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTestsFramework.Framework;

internal static class IntegrationTestsServiceCollectionExtensions
{
    internal static IServiceCollection AddTestJwtAuthentication(this IServiceCollection services)
    {
        return services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = JwtTokenConfigProvider.Issuer,
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = JwtTokenConfigProvider.SecurityKey,
            };
        });
    }

    internal static void AddTestAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
        .AddPolicy("SomePolicyName1", p =>
            p.RequireAssertion(c => c.HasScope(TestConstants.Scopes.WriteMappingScope)))
        .AddPolicy("SomePolicyName2", p =>
            p.RequireAssertion(c => c.HasScope(TestConstants.Scopes.ReadIdentityScope)));
    }

    internal static IServiceCollection AddTestDynamoDBClient(this IServiceCollection services, PlaceholderIAmazonDynamoDB mockDynamoDb)
    {
        services.RemoveAll<PlaceholderIAmazonDynamoDB>();

        return services.AddSingleton(mockDynamoDb);
    }

    internal static IServiceCollection AddInMemoryTestDatabase<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        services.RemoveAll<IDbContextOptionsConfiguration<TDbContext>>();

        return services.AddDbContext<TDbContext>(options =>
        {
            options.UseInMemoryDatabase(TestConstants.Database.TestDBName);
        });
    }

    private static IServiceCollection RemoveAll<TService>(this IServiceCollection services)
    {
        var descriptors = services.Where(d => d.ServiceType == typeof(TService)).ToList();
        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }

        return services;
    }

    private static bool HasScope(this AuthorizationHandlerContext context, string requiredScope)
    {
        var scopeClaim = context.User.FindFirst(OpenIdConnectParameterNames.Scope)?.Value;
        return scopeClaim?.Split(' ').Contains(requiredScope) == true;
    }
}
