# GCop 207

> *"The logic seems extensive. Rename the method to imply this. E.g: Calculate, Find, Select, Create, Evaluate, etc"*

## Rule description

For historic reasons, a method named GetXyz() can be perceived as something that’s simple and can run quickly. As a result, developers may call it liberally and multiple times (for example in loops or Linq methods). 

When your method’s implementation contains substantial logic, you should name the method in a way to bring this to the attention of the callers, for instance to allow them to run it once and store the result in a variable. 

## Example

```csharp
protected string GetImage(int imageID)
{
    //more than 25 statements
}
```

*should be* 🡻

```csharp
protected string FindImage(int imageID)
{
    //more than 25 statements
}
```