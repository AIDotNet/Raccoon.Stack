namespace Raccoon.Stack.Data.Contracts.Options;

public class RaccoonFactoryOptions<TRelationOptions> where TRelationOptions : RaccoonRelationOptions
{
    public List<TRelationOptions> Options { get; set; } = new();
}
