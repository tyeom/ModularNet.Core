using ModularNet.Core.Enums;

namespace ModularNet.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class InjectableAttribute : Attribute
{
    public ServiceScope Scope { get; set; }

    public InjectableAttribute()
    {
        Scope = ServiceScope.Scoped;
    }

    public InjectableAttribute(ServiceScope scope)
    {
        Scope = scope;
    }
}
