# GCop 640

> *"Write it as `foo.Except(VALUE).Any()` "*

## Rule description

The `Contains()` method determines whether an element is in the List. It is an instance method which takes an object, while `Any()` is an extension method which takes a predicate. So if you want to check for a specific condition, use `Any`. If you want to check for the existence of an element, use `Contains`.

## Example

```csharp
var myResult = foo.Any(fo => fo != bar);
```

*should be* 🡻

```csharp
var myResult = foo.Except(bar).Any();
```