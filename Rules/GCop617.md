# GCop 617

> *"Avoid deep nesting of `if` statements. Break the method down into smaller methods, or return early if possible."*

## Rule description

Excessive nesting actually makes logic hard to follow, reduce readability and it makes refactoring code quite tricky.
To avoid this you can break the method into smaller ones, return early `if` or invert logical blocks if they can reduce your nesting.

## Example

```csharp
if (condition) LongMethod();
```

*should be* 🡻

```csharp
if (condition) 
{
    var foo = SimpleMethod();
    if(foo == bar)
        AnotherSimpleMethod();
}
```