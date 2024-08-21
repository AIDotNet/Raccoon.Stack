namespace Raccoon.Stack.Ddd.Domain.Options;

public class AuditEntityOptions
{
    public Type UserIdType { get; set; }

    public AuditEntityOptions()
    {
        UserIdType = typeof(Guid);
    }
}
