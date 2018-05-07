# GCop 100

> *"Replace with Log.Error(...)"*

## Rule description

`Log.Error(...)` is a shortcut to `ApplicationEventManager.RecordException(myException);` and should be used instead.

## Example

```csharp
ApplicationEventManager.RecordException(myException);
```

*should be* 🡻

```csharp
Log.Error(myException);
```
