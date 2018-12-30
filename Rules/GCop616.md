# GCop 616

> *"Reverse your `if` criteria and use `continue`. That will eliminate the need for a big `if` block and make the code more readable."*

## Rule description

You can use the `continue` statement to avoid deeply nested conditional code, or to optimize a loop by eliminating frequently occurring cases that you would like to reject.

## Example

```csharp
foreach (var item in foo)
{
    if(item != null)
    {
        ...
    }
}
```

*should be* 🡻

```csharp
foreach (var item in foo)
{
    if(item == null) continue;
    
    ...
}
```