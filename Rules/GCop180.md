# GCop 180

> *"Double and float comparison isn't exact in .NET. Use myDouble.AlmostEquals(anotherDouble) instead."*

## Rule description

It's a standard problem due to how the computer stores floating point values. For example a float/double can't store 0.1 precisely. [It will always be a little off](https://stackoverflow.com/questions/1398753/comparing-double-values-in-c-sharp). 

That's why using `==` and `!=` operators can give you random results. Instead you should use the `AlmostEquals()` method, which optionally takes another parameter called *accuracy* (by default it's 0.001). It will then return whether the difference between the two compared values is less than or equal to the accepted accuracy.

Note: If you want exact numbers, use the decimal type instead, which stores numbers in decimal notation. Thus 0.1 will be representable precisely.

## Example

```csharp
double commission;

public double Commission
{
    get => commission;    
    set
    {
    	if (commission != value)
    	{
            ...
    	}
    }
}
```

*should be* 🡻

```csharp
double commission;

public double Commission
{
    get => commission;    
    set
    {
    	if (!commission.AlmostEquals(value))
    	{
            ...
    	}
    }
}
```