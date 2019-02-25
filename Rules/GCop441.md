# GCop 441

> *"Use `is` operator instead of `as` operator."*

## Rule description

The `as` operator returns the cast value if the cast can be made successfully. The `is` operator returns only a `Boolean` value. It can therefore be used when you just want to determine an object's type but do not have to actually cast it.

In C# 7.0 and later, use the `is` operator with pattern matching to check the type conversion and cast the expression to a variable of that type in one step.

## Example1

```csharp
private void FooBar(Foo foo)
{
    if (foo.Bar as string != null)
    {
        ...
    }
}
```

*should be* 🡻

```csharp
private void FooBar(Foo foo)
{
    if (foo.Bar is string)
    {
        ...
    }
}
```

## Example2

```csharp
private void FooBar(Foo foo)
{
    if (foo.Bar as string != null)
    {
        var bar = foo.Bar as string;
        ...
    }
}
```

*should be* 🡻

```csharp
private void FooBar(Foo foo)
{
    if (foo.Bar is string bar)
    {
        ...
    }
}
```

*OR* 🡻
```csharp
private void FooBar(Foo foo)
{
    var bar = foo as string;
    if (bar != null)
    {
        ...
    }
}
```
