# GCop 424

> *"This method is redundant. Callers of this method can just call `Foo.Count()` which is as clean."*

## Rule description

There is no need to call a method which just return count of a collection items. Instead You can simply call the `Count()` method on that collection, which is more readable and cleaner.

This warning is shown when the collection property is itself `public`.

## Example

```csharp
public class Bar
{
    public IEnumerable<string> Foo { get; set; }
    
    public int CountFoo()
    {
        return Foo.Count();
    }
    ...
}

var result = new Bar().CountFoo();
```

*should be* 🡻

```csharp
public class Bar
{
    public IEnumerable<string> Foo { get; set; }
    ...
}

var result = new Bar().Foo.Count();
```
