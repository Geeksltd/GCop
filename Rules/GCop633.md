# GCop 633

> *"Instead of hard-coding the boundary check, set the boundary of the NumericProperty in its M# definition."*

## Rule description
In M#, for numeric properties, you can define the minimum and maximum allowed values in their entity definition. That will not only  generate the necessary validation logic in the entity class, but also in the UI forms.

## Example

```csharp
public override async Task Validate()
{
    await base.Validate();
    
    if (myNumericProperty < myValue) 
        throw new ValidationException(...);
    ...
}
```

*should be* 🡻

```
   added to the M# entity definition
```
