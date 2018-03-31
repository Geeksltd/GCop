# GCop172

> *"Remove the check for null. Instead use NullableObject?.Invoke()"*


## Rule description
In C#6 code using the null conditional operator indicating that this code will not throw a NullReferenceException exception if handler is null, which avoid you writing null checks that you would have to do in previous versions of the C# language.

## Example 1
```csharp
Delegate handler = null;
if (handler != null)
{
    handler.Invoke();
}
```
*should be* 🡻

```csharp
Delegate handler = null;
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


