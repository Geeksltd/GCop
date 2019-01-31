# GCop 630

> *"Override the Validate() method and write your validation logic there."*

## Rule description

For adding validation criteria we should override `Validate()` method and add our custom logics there. There is no need to write validation logics in other methods.

Note: This rule is sensitive to invoking a method which has the word *Validate* in its name.

## Example

```csharp
protected override async Task OnSaving(CancelEventArgs e)
{
    await base.OnSaving(e);
    
    if (SomeValidateMethod())
        ...;
    ...
}
```

*should be* 🡻

```csharp
public override async Task Validate()
{ 
    await base.Validate();
    
    if (SomeValidateMethod())
        ...;
}
```
