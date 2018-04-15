# GCop 172

> *"Remove the check for null. Instead use NullableObject?.Invoke()"*

## Rule description

In C#, from version 6, the `?.` expression can be used to simplify the code.

## Example 1

```csharp
Delegate handler = null;
...
if (handler != null)
{
    handler.Invoke();
}
```

*should be* 🡻

```csharp
Delegate handler = null;
...
handler?.Invoke();
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
