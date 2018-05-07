# GCop 631

> *"All validation logic should be written inside Validate() method. OnValidating is meant to be used for special cases such as setting late-bound default values."*

## Rule description

For adding validation criteria you should override `Validate()` method and add your custom logics there. `OnValidating` is meant to be used for special cases such as setting late-bound default values.

## Example

```csharp
protected override Task OnValidating(EventArgs e)
{
    ...
    this.ValidateSomeProperty();
    ...
}
```

*should be* 🡻

```csharp
public override async Task Validate()
{
    ...
    this.ValidateSomeProperty();
    ...
}
```