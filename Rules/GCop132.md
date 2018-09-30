# GCop 132

> *"Since the type is inferred, use `var` instead"*

## Rule description

The potential benefit of using `var` instead of type is in readability and brevity. When the type of the variable is clear on the right side of the assignment (e.g. via a cast or a constructor call), there is no benefit of also having it on the left side.

There is one exception which is where the type is a delegate type such as `Action` or `Func`.
 
## Example 1

```csharp
public void Bar()
{
    object foo = new object();
    ...
}
```

*should be* 🡻

```csharp
public void Bar()
{
    var foo = new object();
    ...
}
```
 
## Example 2

```csharp
public void Bar()
{
    Dictionary<int, List<Foo>> dictionary = new Dictionary<int, List<Foo>>();
}
```

*should be* 🡻

```csharp
public void Bar()
{
    var dictionary = new Dictionary<int, List<Foo>>();
}
```