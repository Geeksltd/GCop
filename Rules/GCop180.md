# GCop180

> *"Double and float comparison isn't exact in .NET. Use myDouble.AlmostEquals(anotherDouble) instead."*


## Rule description
...

## Example 1
```csharp
private double _Commission;

public double Commission
{
    get
    {
    	return this._Commission;
    }
    set
    {
    	if (this._Commission != value)
    	{
            ...
    	}
    }
}
```
*should be* 🡻

```csharp
private double _Commission;

public double Commission
{
    get
    {
    	return this._Commission;
    }
    set
    {
    	if (this._Commission.AlmostEquals(value))
    	{
            ...
    	}
    }
}
```
