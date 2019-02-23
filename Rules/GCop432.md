# GCop 432

> *"Unnecessary parenthesis should be removed."*

## Rule description

It is possible in C# to insert parenthesis around virtually any type of expression, statement, or clause, and in many situations use of parenthesis can greatly improve the readability of the code. However, excessive use of parenthesis can have the opposite effect, making it more difficult to read and maintain the code.

## Example 1

```csharp
public int MyMethod()
{
    var localItem = 100;
    ...
    return (localItem * 10);
}
```

*should be* 🡻

```csharp
public int MyMethod()
{
    var localItem = 100;
    ...
    return localItem * 10;
}
```

## Example 2

```csharp
public void MyMethod(object arg, int myVar)
{
    int x = (5 + myVar);
    var localItem = ((MyObjectName)(arg));
}
```

*should be* 🡻

```csharp
public void MyMethod(object arg, int myVar)
{
    int x = 5 + myVar;
    var localItem = (MyObjectName)arg;
}
```

## Example 3

```csharp
public void MyMethod()
{
    if ((IsLocalFileSystemWebService(Url) == true))
    {
        ...
    }
}
```

*should be* 🡻

```csharp
public void MyMethod()
{
    if (IsLocalFileSystemWebService(Url) == true)
    {
        ...
    }
}
```