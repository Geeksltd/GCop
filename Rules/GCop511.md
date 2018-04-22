# GCop 511

> *"Either remove the parameter documentation node, or describe it properly."*

## Rule description

The `<param>` tag should be used in the comment for a method declaration to describe one of the parameters for the method. The text for the `<param>` tag will be displayed in IntelliSense, the Object Browser, and in the Code Comment Web Report, so it should be descriptive enough for caller.

## Example

```csharp
///<param name="myParam">the myParam</param>
public void MyMethod(int myParam)
{
    ...
}
```

*should be* 🡻

```csharp
///<param name="myParam">Some actual description....</param> 
public void MyMethod(int myParam)
{
    ...
}
```

*OR* 🡻
```csharp
public void MyMethod(int myParam)
{
    ...
}
```