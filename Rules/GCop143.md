# GCop 143

> *"First line of OnSaved() method must be a call to base.OnSaved() otherwise CachedReferences will have a problem."*

## Rule description

You can call a method in a base class using the base keyword. You can do this from anywhere within the child class, but  it’s a common pattern to call the base class method when you override it.  This allows you to extend the behavior of that method.

## Example

```csharp
protected override async Task OnSaved(SaveEventArgs e)
{
    if (SomeCondition){...}
    ...
}
```

*should be* 🡻

```csharp
protected override async Task OnSaved(SaveEventArgs e)
{
    await base.OnSaved(e);
    if (SomeCondition){...}
    ...
}
```