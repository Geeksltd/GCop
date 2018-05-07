# GCop 167

> *"OnSaving() is called after Validate() and property values set in OnSaving() won't be validated. Instead use OnValidating() to write your prep logic."*
> 
> *"No properties should be set in Validate(). Instead use OnValidating() to write your assignment."*

## Rule description

Sometimes you need to write logic to apply some default values, or other modifications in order to make your object compliant with your validation rules automatically.

Rather than writing those things inside `OnSaving()` or `Validate()` you should write them inside the `OnValidating()` method which is called before `OnSaving` and `Validate` are even called.

## Example1

```csharp
protected override async Task OnSaving(CancelEventArgs e)
{
    this.MyProperty = "Something";
    await base.OnSaving(e);
}
```

*should be* 🡻

```csharp
protected override Task OnValidating(EventArgs e)
{
    this.MyProperty = "Something";
}
```

## Example2

```csharp
public override async Task Validate()
{
    if (this.StartDate == default(DateTime))
    {
        this.StartDate = LocalTime.Now;
    }
    await base.Validate();
}
```

*should be* 🡻

```csharp
protected override Task OnValidating(EventArgs e)
{
    if (this.StartDate == default(DateTime))
    {
        this.StartDate = LocalTime.Now;
    }
}
```
