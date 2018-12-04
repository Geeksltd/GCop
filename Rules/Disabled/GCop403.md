# GCop 403

> *"Change \{PropertyName} to an auto property"*

## Rule description

In C# 3.0 and later, auto-implemented properties make property-declaration more concise when no additional logic is required in the property accessors. They also enable client code to create objects. When you declare a property as shown in the following example, the compiler creates a private, anonymous backing field that can only be accessed through the property's `get` and `set` accessors.

## Example

```csharp
private int myProperty;
public int MyProperty
{
    get { return myProperty; }
    set { myProperty = value; }
}
```

*should be* 🡻

```csharp
public int MyProperty { get; set; }
```