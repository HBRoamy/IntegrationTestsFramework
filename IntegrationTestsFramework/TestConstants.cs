namespace IntegrationTestsFramework;

internal static class TestConstants
{
    internal static class Jwt
    {
        internal const string TestClientId = "Test-JWT-M2M-Token-Client-Id";
        internal const string TestSecret = "Framework_Test_Secret";
        internal const string TestIssuer = "Framework_Test_Issuer";
    }

    internal static class Database
    {
        internal const string TestDBName = "Framework_Test_DB";
    }

    internal static class DynamoDb
    {
        internal static class ConfigKeys
        {
            internal const string Region = "DynamoDb:Region";
            internal const string RoleArn = "DynamoDb:RoleArn";
            internal const string TableName = "DynamoDb:TableName";
        }

        internal static class ConfigValues
        {
            internal const string Region = "us-east-1";
            internal const string RoleArn = "arn:aws:iam::123456789012:role/CrossAccountTestRole";
            internal const string TableName = "test-TableName";
        }
    }

    internal static class Scopes
    {
        internal const string WriteMappingScope = "scope.write";
        internal const string ReadIdentityScope = "scope.read";
    }
}
