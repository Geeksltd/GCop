# GCop112

> *"This class is too large. Break its responsibilities down into more classes."*


## Rule description
When classes get too big, it is likely they have too many responsibilities and it is against SOLID first rule which is having a *Single responsibility*. 

Consider breaking the class into two or more classes, each with specific responsibilities.

Your ultimate goal should be to achieve a highly cohesive set of small classes.

## Example 1
```csharp
public class BigClass
{
    // Multiple methods, and more than 1000 lines
}
```
*should be* 🡻

```csharp
public class SmallClass1
{
    // ...
}
```

