# GCop 441

> *"Use `is` operator instead of `as` operator."*

## Rule description

The `as` operator returns the cast value if the cast can be made successfully. The `is` operator returns only a `Boolean` value. It can therefore be used when you just want to determine an object's type but do not have to actually cast it.

## Example

```csharp
private void Bar(object foo)
{
    if (foo as string != null)
    {
        ...
    }
}
```

*should be* 🡻

```csharp
private void Bar(object foo)
{
    if (foo is string)
    {
        ...
    }
}
```
