using System;
using System.Collections.Generic;
using System.Text;

namespace ModularNet.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class PostAttribute : Attribute
{
    public string Path { get; set; }

    public PostAttribute(string path = "")
    {
        Path = path;
    }
}
