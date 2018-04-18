# GCop 627

> *"Instead of « ?? false » use the more readable expression of « == true »"*
> 
> *"Instead of !(nullable expression == true) use the more readable alternative of: (nullable expression == false)."*

## Rule description

Human brain can understand positive phrases better than negative ones so it is recommended to use `==` rather than `!=` where possible, to have a more readable code.

When dealing with expressions that are `Nullable<bool>`, using `?? false` is the same as using `== true` whether the expression's value is `true`, `false` or `null`. But `== true` is more readable as it's *positive logic*.

## Example1

```csharp
var nullableBool = GetSomeNullableBool(...);
...
if(nullableBool ?? false)
{
   ...
}
```

*should be* 🡻

```csharp
var nullableBool = GetSomeNullableBool(...);
...
if(nullableBool == true)
{
   ...
}
```

## Example2

```csharp
if (!nullableBool == true)
{
   ...
}
```

*should be* 🡻

```csharp
if (nullableBool == false)
{
   ...
}
```
