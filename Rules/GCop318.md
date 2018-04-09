# GCop 318

> *"This will cause the query to be computed multiple times. Instead call .ToList() on the variable declaration line to avoid unwanted extra processing."*

## Rule description

An object of type IEnumerable can be resource consuming every time that it's iterated in foreach loops or Linq methods that evaluate the result such as ToList(), ToArray(), Count(), Any(), ...

If you need the result of an IEnumerable object more than once, it's better to call .ToArray() or .Tolist() on it once to prepare the final result, and then use that directly.

## Example

```csharp
private void MyMethod()
{
    var children = categories.Where(lai => lai.SomethingThatIsTimeConsuming() == ...);
    
    if (children.Count() == ...) // Running Count() requires the above lambda expression to get executed for every item.
    {
        ...
    }
    
    foreach (var Child in children) // Again the lambda expression is executed for every item.
    {
        ...
    }
}
```

*should be* 🡻

```csharp
private void MyMethod()
{
    var children = allcategories.Where(lai => lai.SomethingThatIsTimeConsuming() == ...);
                   .ToArray(); // the lambda expression is executed only once per item and the result is stored.
    
    if (children.Count() == ...)
    {
        ...
    }
    
    foreach (var Child in children)
    {
        ...
    }
}
```