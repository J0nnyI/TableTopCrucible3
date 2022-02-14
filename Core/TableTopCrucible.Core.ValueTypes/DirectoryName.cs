namespace TableTopCrucible.Core.ValueTypes;

/// <summary>
///     the name of a directory without a path
/// </summary>
public class DirectoryName : ValueType<string, DirectoryName>
{
    public Name ToName() => Name.From(Value);
}