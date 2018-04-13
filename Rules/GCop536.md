# GCop 536

> *"Remove empty xml node documentation"*

## Rule description

There's no value to write empty XML documentation comments. They just make your dirty and unreadable.

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
///<param name="myParam">description</param> 
public void MyMethod(int myParam)
{
    ...
}
```