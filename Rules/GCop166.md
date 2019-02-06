# GCop 166

> *"Avoid assignment within conditional statements"*

## Rule description

Using assignment within conditional statements would decrease readability. Instead you should use separate statements for the assignment, and the comparison.

## Example 1

```csharp
public void MyMethod()
{
    if ((foo = items.Count()) < 1)
    {
        ...
    }
}
```

*should be* 🡻

```csharp
public void MyMethod()
{
    var foo = items.Count();
    if (foo < 1)
    {
        ...
    }
}
```

## Example 2

```csharp
public void MyMethod()
{
    if ((TotalRows = Query.Count() == 2)
        ...
}
```

*should be* 🡻

```csharp
public void MyMethod()
{
    TotalRows = Query.Count(); 
    if (TotalRows == 2)
        ...
}
```
