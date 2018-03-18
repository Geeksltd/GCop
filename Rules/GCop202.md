# GCop202

> *"Don’t end the name of **methods** with the same name as the **class**"*
> 
> *"Don’t end the name of **enum members** with the same name as the **enum**"*


## Rule description
Mostly we consider ending the name of derived classes with the name of the base class. 
So methods with the same end with class, reduce readability of the program.
Static Methodes are expected from this rule.
## Example 1
```csharp
public class SampleClass
{
    public void BindSampleClass()
    {
        //several lines of code
    }
}
```
*should be* 🡻

```csharp
public class SampleClass
{
    public void Bind()
    {
        //several lines of code
    }
}
```
