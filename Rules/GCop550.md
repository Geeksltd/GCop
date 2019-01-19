# GCop 550

> *"Return `Task.FromResult` instead of returning `null`"*

## Rule description

The `Task.FromResult<T>(null)` method is commonly used to generate a task whose result is null. Returning null from a non-async `Task/Task<T>` method will cause a `NullReferenceException` at runtime. This problem can be avoided by returning `Task.FromResult<T>(null)` instead.
## Example1

```csharp
Task<foo> Bar()
{
    return null;
}
```

*should be* 🡻

```csharp
Task<foo> Bar()
{
    return Task.FromResult<foo>(null);
}
```

## Example2

```csharp
Task<foo> Bar()
{
    return foo?.FooBar();
}
```

*should be* 🡻

```csharp
Task<foo> Bar()
{
    if (foo != null)
    {
        return foo.FooBar();
    }
    else
    {
        return Task.FromResult<foo>(null);
    }
}
```