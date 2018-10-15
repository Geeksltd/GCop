# GCop 644

> *"Remove redundant base interface."*

## Rule description

If the base class contains a member that matches an interface member, the base class member can work as the implementation of the interface member and you are not required to manually implement it again. So there is no need to use that interface again.

## Example

```csharp
public class Foo<T> : List<T>, IEnumerable<T>
{
    ...
}
```

*should be* 🡻

```csharp
public class Foo<T> : List<T>
{
    ...
}
```
