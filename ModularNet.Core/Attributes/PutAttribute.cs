namespace ModularNet.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class PutAttribute : Attribute
{
    public string Path { get; set; }

    public PutAttribute(string path = "")
    {
        Path = path;
    }
}
