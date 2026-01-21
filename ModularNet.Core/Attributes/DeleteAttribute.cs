namespace ModularNet.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class DeleteAttribute : Attribute
{
    public string Path { get; set; }

    public DeleteAttribute(string path = "")
    {
        Path = path;
    }
}
