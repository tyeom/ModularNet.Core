using System;
using System.Collections.Generic;
using System.Text;

namespace ModularNet.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class GetAttribute : Attribute
{
    public string Path { get; set; }

    public GetAttribute(string path = "")
    {
        Path = path;
    }
}
