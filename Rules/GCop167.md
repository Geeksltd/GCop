# GCop 167

> *"OnSaving() is called after Validate() and property values set in OnSaving() won't be validated. Instead use OnValidating() to write your prep logic."*
> 
> *"No properties should be set in Validate(). Instead use OnValidating() to write your assignment."*

## Rule description

You can add required business logic according to the requirements to the `OnValidating()`. science `OnSaving` and `Validate` are called after `OnValidating`, all assignments should be written in `OnValidating` method.

## Example1

```csharp
protected override Task OnSaving(CancelEventArgs e)
{
    this.MyProperty = "Something";
    ...
}
```

*should be* 🡻

```csharp
protected override Task OnValidating(EventArgs e)
{
    this.MyProperty = "Something";
    ...
}
```

## Example2

```csharp
public override Task Validate()
{
    if (this.StartDate == default(DateTime))
    {
        this.StartDate = LocalTime.Now;
    }
    ...
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
    ...
}
```