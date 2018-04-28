# GCop 159

> *"In OnSaved method the property IsNew must not be used, instead use: e.Mode == SaveMode.Insert"*

## Rule description

When you have to perform an operation on an instance in some cases you need to know whether it is a new or an existing instance. To do so, M# provides you the readonly boolean property IsNew.

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