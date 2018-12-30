# GCop 614

> *"`Random` instance should be defined and instantiated as a `static` class field."*

## Rule description

Every time new `Random()` is called, it is initialized using the clock. This means that in a tight loop it returns same value lots of times. So keep a single `Random` instance and keep using `Next` on the same instance is a best practice.

## Example

```csharp
public class Foo
{
    void Bar()
    {
        var rnd = new Random();
        rnd.Next(1,100);
    }
}
```

*should be* 🡻

```csharp
public class Foo
{
    static Random Random = new Random();
    void Bar()
    {
        Random.Next(1,100);
    }
}
```