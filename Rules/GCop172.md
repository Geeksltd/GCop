# GCop 172

> *"Remove the check for `null`. Instead use `NullableFoo?.Invoke()`"*

## Rule description

In C#, from version 6, the `?.` expression can be used to simplify the code.

## Example 1

```csharp
public delegate void LogHandler(string message);
public event LogHandler Log;

protected void OnLog(string message)
{
    if (Log != null)
    {
        Log(message);
    }
}
```

*should be* 🡻

```csharp
public delegate void LogHandler(string message);
public event LogHandler Log;

protected void OnLog(string message)
{
    Log?.Invoke(message);  
}
```

## Example 2

```csharp
public void OnXYZ(SomeEventArgs e)
{
    var evt = XYZ;
    if (evt != null)
        evt(sender, e);
}
```

*should be* 🡻

```csharp
public void OnXYZ(SomeEventArgs e) => XYZ?.Invoke(sender, e);
```
