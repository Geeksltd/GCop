# GCop 626

> *"The condition is unnecessary."*

## Rule description

A call to `Append(...)` method of a `StringBuilder` object will be ignored if the parameter is `null` or `empty string`. Therefore it's unnecessary to add such logic in your code.

## Example

```csharp
public void FooBar(string foo)
{           
    var bar = new StringBuilder();             
    if (foo.HasValue())
       bar.Append(foo);
}
```

*should be* 🡻

```csharp
public void FooBar(string foo)
{           
    var bar = new StringBuilder();             
    bar.Append(foo);
}
```
