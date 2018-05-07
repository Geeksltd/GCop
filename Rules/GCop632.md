# GCop 632

> *"Use OnValidating() for setting late-bound properties. Validate() should only be used for validation, without changing the object state."*

## Rule description

The `Validate()` method should only be used for validation, without changing the object state. You can override `OnValidating()` method to implement required business logic according to your requirements.



## Example

```csharp
public override async Task Validate()
{
    ...
    this.SetSomeProperty();
    ...
}
```

*should be* 🡻

```csharp
protected override Task OnValidating(EventArgs e)
{
    ...
    this.SetSomeProperty();
    ...
}
```
