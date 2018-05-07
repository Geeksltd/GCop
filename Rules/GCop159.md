# GCop 159

> *"In OnSaved method, the property `IsNew` must not be used, instead use: e.Mode == SaveMode.Insert"*

## Rule description

The `IsNew` property is always `false` when an object is saved. On the other hand when the `OnSaved()` method is running, the object is always saved in the database. Therefore in an `OnSaved()` method `IsNew` is always `false` and it doesn't make sense to write conditional logic based on it.

Instead, the `Mode` property of the event args object should be used for the conditional logic.

## Example

```csharp
protected override async Task OnSaved(SaveEventArgs e)
{
    await base.OnSaved(e);
    if (IsNew)
    {
        ...
    }
}
```

*should be* 🡻

```csharp
protected override async Task OnSaved(SaveEventArgs e)
{
    await base.OnSaved(e);
    if (e.Mode == SaveMode.Insert)
    {
        ...
    }
}
```
