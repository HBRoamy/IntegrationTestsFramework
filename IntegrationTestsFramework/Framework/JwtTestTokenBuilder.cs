using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IntegrationTestsFramework.Framework;

internal class JwtTestTokenBuilder
{
    private readonly List<Claim> _claims;
    private int _expiresInMinutes;

    private JwtTestTokenBuilder() 
    {
        _claims = [];
        _expiresInMinutes = 30;
    }

    internal static JwtTestTokenBuilder Initialize()
    {
        return new JwtTestTokenBuilder();
    }

    internal JwtTestTokenBuilder WithScopes(params string[] scopes)
    {
        AddClaim(OpenIdConnectParameterNames.Scope, string.Join(" ", scopes));
        return this;
    }

    internal JwtTestTokenBuilder WithExpiration(int expiresInMinutes)
    {
        _expiresInMinutes = expiresInMinutes;
        return this;
    }

    internal string Build()
    {
        var token = new JwtSecurityToken(
            issuer: JwtTokenConfigProvider.Issuer,
            claims: _claims,
            expires: DateTime.UtcNow.AddMinutes(_expiresInMinutes),
            signingCredentials: JwtTokenConfigProvider.SigningCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private void AddClaim(string type, string value)
    {
        _claims.Add(new Claim(type, value));
    }
}
