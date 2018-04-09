# GCop 153

> *"Instead use IEnumerable< T >.Lacks(T item)"*
>
> *"Instead use IEnumerable< T >.Lacks(T item, bool caseSensitive)"*

## Rule description

Human brain can understand positive expressions and statements faster than negative ones. To improve code readability it is better to use `Lacks` rather than `!Contains`. 

## Example

```csharp
if (!myStringList.Contains(myVar))
{
    ...
}
```

*should be* 🡻

```csharp
if (myStringList.Lacks(myVar))
{
    ...
}
```