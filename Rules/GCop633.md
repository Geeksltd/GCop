# GCop 633

> *"Instead of hard-coding the boundary check, set the boundary of the NumericProperty in its M# definition."*

## Rule description

...

## Example

```csharp
public override async Task Validate()
{
    ...
    if (myNumericProperty < myValue) throw new ValidationException(...);
    ...
}
```

*should be* 🡻

```csharp
(...corrected version)
```