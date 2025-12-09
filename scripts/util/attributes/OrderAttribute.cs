using System;
using System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class OrderAttribute : Attribute
{
    public OrderAttribute([CallerLineNumber] int order = 0)
    {
        Order = order;
    }

    public int Order { get; }
}
