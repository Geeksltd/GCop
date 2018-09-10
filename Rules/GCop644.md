# GCop 644

> *"Remove redundant base interface."*

## Rule description

...

## Example

```csharp
public class TestClass<T> : List<T>, IEnumerable<T>
{
    ...
}
```

*should be* 🡻

```csharp
public class TestClass<T> : List<T>
{
    ...
}
```
