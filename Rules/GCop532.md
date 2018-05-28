# GCop 532

> *"It should be written as `{CollectionName}.Lacks({SomeValue})`"*

## Rule description

The `Lacks()` method determines if none of the items in this list meet a given criteria. It works as opposite of `Contains()`. It is more readable than `None()` method.

## Example

```csharp
var result = List.None(rec => rec == Something);
```

*should be* 🡻

```csharp
var result = List.Lacks(Something);
```