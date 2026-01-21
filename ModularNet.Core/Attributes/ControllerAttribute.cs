using System;
using System.Collections.Generic;
using System.Text;

namespace ModularNet.Core.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ControllerAttribute : Attribute
{
    public string Route { get; set; }

    public ControllerAttribute(string route = "")
    {
        Route = route;
    }
}
