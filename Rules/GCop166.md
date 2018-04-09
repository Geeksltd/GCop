# GCop 166

> *"Avoid assignment within conditional statements"*

## Rule description

Using assignment within conditional statements would decrease readability. Instead you should use seperate statements for the assignment, and the comparison.

## Example 1

```csharp
public void MyMethod()
{
    if ((TotalRows = Query.Count()) < 1)
        ...
}
```

*should be* 🡻

```csharp
public void MyMethod()
{
    TotalRows = Query.Count();
    if (TotalRows < 1)
        ...
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
