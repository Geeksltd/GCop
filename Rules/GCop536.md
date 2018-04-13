# GCop 536

> *"Remove empty xml node documentation"*

## Rule description

Empty XML documentation parameters are just unnecessary noise and should be removed.

## Example

```csharp
///<param name="myParam"></param>
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
