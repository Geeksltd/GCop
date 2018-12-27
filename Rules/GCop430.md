# GCop 430

> *"Use `foo.HasValue` as negative logic is taxing on the brain."*

## Rule description

Human brain can understand positive expressions and statements faster than negative ones. To improve code readability it is better to use *`HasValue`* rather than *`!= null` for `Nullable<T>` object*. 

## Example

```csharp
if (foo != null)
{
     ...
}
```

*should be* 🡻

```csharp
if (foo.HasValue)
{
    ...
}
```
