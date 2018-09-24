# GCop 657

> *"Simplify nested using statement."*

## Rule description

Nested `using` statements can be maid more readable and prettier by only using one pair of braces after the last `using` statement.

## Example

```csharp
using (var fileStream = new FileStream("path", FileMode.OpenOrCreate))
{ 
    using (var stramReader = new StreamReader(fileStream))
    {
    }
}
```

*should be* 🡻

```csharp
using (var fileStream = new FileStream("path", FileMode.OpenOrCreate))
using (var stramReader = new StreamReader(fileStream))
{
}
```

