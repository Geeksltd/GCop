# GCop 319

> *"[Database.Save(this);] without condition will create a loop in \{OnSaved / OnSaving} method."*

## Rule description

...

## Example

```csharp
protected override async Task OnSaved(SaveEventArgs e)
{
    await base.OnSaved(e);
    ...
    await Database.Save(this);
}
```

*should be* 🡻

```csharp
protected override async Task OnSaved(SaveEventArgs e)
{
    await base.OnSaved(e);
    ...
    if(SomeCondition)
        await Database.Save(this);
}
```