# GCop 630

> *"Override the Validate() method and write your validation logic there."*

## Rule description

For adding validation criteria we should override Validate() method and add our custom logics there. There is no need to write validation logics in other methods.

## Example

```csharp
protected override Task OnSaving(CancelEventArgs e)
{
    ...
    var xx = ValidateMethod();
    ...
}
```

*should be* 🡻

```csharp
public override Task Validate()
{
    //...Validation logic
    return base.Validate();
}
```