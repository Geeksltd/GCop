# GCop 100

> *"Replace with Log.Error(...)"*

## Rule description

The `RecordException` method formats information contained in the exception object (including all inner exceptions) and logs it with the title of “Exception” in the database. 

## Example

```csharp
ApplicationEventManager.RecordException(myException);
```

*should be* 🡻

```csharp
Log.Error(myException);
```