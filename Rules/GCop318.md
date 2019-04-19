# GCop 318

> *"This will cause the query to be computed multiple times. Instead call `.ToList()` on the variable declaration line to avoid unwanted extra processing."*

## Rule description

An object of type `IEnumerable` can be resource consuming every time that it's iterated in foreach loops or Linq methods that evaluate the result such as `ToList()`, `ToArray()`, `Count()`, `Any()`, ...

If you need the result of an `IEnumerable` object more than once, it's better to call `.ToArray()` or `.Tolist()` on it once to prepare the final result, and then use that directly.

## Example 1

```csharp
private void Test(IEnumerable<Bar> bar)
{
    var items = bar.Where(lai => lai.Foo == "foo" );
    foreach (var item in items)
    {
        ...
    }
    ...
    foreach (var foo in items)
    {
        ...
    }
}
```

*should be* 🡻

```csharp
private void Test(IEnumerable<Bar> bar)
{
    var items = bar.Where(lai => lai.Foo == "foo" ).Tolist();
    foreach (var item in items)
    {
        ...
    }
    ...
    foreach (var foo in items)
    {
        ...
    }
}
```

## Example 2

```csharp
private void Test(IEnumerable<Bar> bar)
{
    var items = bar.Where(lai => lai.Foo == "foo");
    
    if (items.Count() == ...) // Running Count() requires the above lambda expression to get executed for every item.
    {
        ...
    }
    
    foreach (var item in items) // Again the lambda expression is executed for every item.
    {
        ...
    }
}
```

*should be* 🡻

```csharp
private void Foo()
{
    ...
    var items = bar.Where(lai => lai.Foo == "foo").ToArray(); // the lambda expression is executed only once per item and the result is stored.
    
    if (items.Count() == ...)
    {
        ...
    }
    
    foreach (var item in items)
    {
        ...
    }
}
```