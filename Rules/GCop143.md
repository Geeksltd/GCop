# GCop 143

> *"First line of OnSaved() method must be a call to base.OnSaved() otherwise CachedReferences will have a problem."*

## Rule description

When overriding event handler methods in entity classes, you should invoke the base implementation. Otherwise you can accidentally skip any event handler code written in the parent class..

## Example

```csharp
protected override async Task OnSaved(SaveEventArgs e)
{
    if (SomeCondition) {...}
    ...
}
```

*should be* 🡻

```csharp
protected override async Task OnSaved(SaveEventArgs e)
{
    await base.OnSaved(e);
    
    if (SomeCondition) {...}
    ...
}
```
