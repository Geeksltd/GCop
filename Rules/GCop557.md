# GCop {number}

> *"Use `cs()` method instead of the `c#:` prefix"*

## Rule description

The `cs()` method is an alternative to adding the `c#:` prefix. It is more readable and easy to use than "c#:".

## Example

```csharp
var foo = Link("c#:item.Number");
```

*should be* 🡻

```csharp
var foo = Link(cs("item.Number"));
```