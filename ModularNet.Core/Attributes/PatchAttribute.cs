namespace ModularNet.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class PatchAttribute : Attribute
{
    public string Path { get; set; }

    public PatchAttribute(string path = "")
    {
        Path = path;
    }
}
