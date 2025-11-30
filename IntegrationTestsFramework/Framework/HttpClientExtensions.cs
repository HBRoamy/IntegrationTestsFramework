using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http.Headers;

namespace IntegrationTestsFramework.Framework;

internal static class HttpClientExtensions
{
    /// <summary>
    /// Sets a JWT Bearer token with the specified scopes on the HttpClient's default request headers.
    /// This method clears any existing authorization header before setting the new one to ensure test isolation.
    /// </summary>
    /// <param name="client">The HttpClient instance to configure.</param>
    /// <param name="expiresInMinutes">The number of minutes until the token expires. Defaults to 10 minutes.</param>
    /// <param name="scopes">The scopes to include in the JWT token.</param>
    /// <returns>The same HttpClient instance for method chaining.</returns>
    internal static HttpClient WithJwtBearerM2MTokenIncludingScopes(this HttpClient client, int expiresInMinutes = 10, params string[] scopes)
    {
        var jwtToken = JwtTestTokenBuilder
            .Initialize()
            .WithScopes(scopes)
            .WithExpiration(expiresInMinutes)
            .Build();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtToken);
        return client;
    }

    /// <summary>
    /// Resets the authorization header of the HttpClient to null.
    /// </summary>
    /// <param name="client">The HttpClient instance to configure.</param>
    internal static HttpClient WithoutAuthHeader(this HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
        return client;
    }
}