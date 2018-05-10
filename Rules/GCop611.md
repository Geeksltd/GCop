# GCop 611

> *"It should be written as `Database.Reload({ParameterName})`."*

## Rule description
If you want to obtain the database version of an entity (that is changed in memory), use `Database.Reload(...)` to be more explicit about your intention.

## Example

```csharp
protected override void OnSaving(SaveEventArgs e)
{
    if (e.Mode == SaveMode.Update)
    {
        var original = Database.Get<Customer>(this.ID);
        if (original.Email != this.Email)
        {
            // Email is changed, do something...
        }
    }
}
```

*should be* 🡻

```csharp
protected override void OnSaving(SaveEventArgs e)
{
    if (e.Mode == SaveMode.Update)
    {
        var original = Database.Reload(myCustomer);
        if (original.Email != this.Email)
        {
            // Email is changed, do something...
        }
    }
}
```
