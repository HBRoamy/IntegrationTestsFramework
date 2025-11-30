using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace IntegrationTestsFramework.Framework;

internal static class JwtTokenConfigProvider
{
    public static string Issuer { get; } = TestConstants.Jwt.TestIssuer;

    public static SecurityKey SecurityKey { get; } =
        new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(TestConstants.Jwt.TestSecret)
        );

    public static SigningCredentials SigningCredentials { get; } =
        new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

    internal static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();
}
