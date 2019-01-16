# GCop 154

> *"Should be written simply as `foo.Except(bar)`"*

## Rule description

The `Lacks` method work as opposite of `Contains()`. It gets all items of this list except those meeting a specified criteria , which is exactly what `Except` does. It is more readable and easy to understand to use `Except` rather than `Lacks`.

## Example1

```csharp
var myResult = foo.Where(s => bar.Lacks(s));
```

*should be* 🡻

```csharp
var myResult = foo.Except(bar);
```

## Example2

```csharp
var myResult = foo.Except(s => bar.Lacks(s));
```

*should be* 🡻

```csharp
var myResult = foo.Except(bar);
```
