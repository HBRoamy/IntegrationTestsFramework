namespace IntegrationTestsFramework.Placeholders
{
    public class PlaceholderEntity(Guid uuid, Guid? userId = null)
    {
        public Guid UUId { get; } = uuid;
        public Guid? UserId { get; } = userId;
    }
}
