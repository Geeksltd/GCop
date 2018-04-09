# GCop 647

> *"Shorten this property by defining it as expression-bodied."*

## Rule description

Where the body of a property getter is a small single line, it can be written in a briefer format.

## Example

```csharp
public string Property
{
    get { return "something"; }
}
```

*should be* 🡻

```csharp
public string Property => "something";
```