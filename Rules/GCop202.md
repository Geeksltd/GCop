# GCop202

> *"Don’t end the name of **methods** with the same name as the **class**"*
> 
> *"Don’t end the name of **enum members** with the same name as the **enum**"*


## Rule description
As instance methods are defined inside a class, the name of the class is already implied in their context and should not be repeated in the name of the method. 
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
