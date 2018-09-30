# GCop 100

> *"Replace with `Log.Error(...)`"*

## Rule description

`Log.Error()` is a shortcut to `ApplicationEventManager.RecordException()` and should be used instead.

## Example

```csharp
ApplicationEventManager.RecordException(fooException);
```

*should be* 🡻

```csharp
Log.Error(fooException);
```