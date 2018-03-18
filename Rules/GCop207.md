# GCop207

> *"The logic seems extensive. Rename the method to imply this. E.g: Calculate, Find, Select, Create, Evaluate, etc"*


## Rule description
If a method name start with **Get** keyword and continue with a camelcase word and count of body statements are more than 25 statements, this will be hint.

Actually if a method role is getting value, there is no need to have a long body.
It seems that long body is more needed for finding a value or create an object or etc.

## Example 1
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
