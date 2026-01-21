using System;
using System.Collections.Generic;
using System.Text;

namespace ModularNet.Core.Attributes;

public class ModuleAttribute : Attribute
{
    public Type[] Imports { get; set; } = Array.Empty<Type>();
    public Type[] Controllers { get; set; } = Array.Empty<Type>();
    public Type[] Providers { get; set; } = Array.Empty<Type>();
}
