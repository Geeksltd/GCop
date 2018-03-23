# GCop138

> *"When you catch an exception you should throw exception or at least log error"*


## Rule description
Place throw statements so that the stack trace will be helpful.
The stack trace begins at the statement where the exception is thrown and ends at the catch statement that catches the exception.

Also callers should be able to assume that there are no side effects when an exception is thrown from a method, so return null or an empty string is not an appropriate solution.

## Example 1
```csharp
public string MyMethod()
{
    try
    {
        //several lines of code
    }
    catch
    {
        return "";
    }
}
```
*should be* 🡻

```csharp
public string MyMethod()
{
    try
    {
        //several lines of code
    }
    catch
    {
        throw;
    }
}
```

## Example 2
```csharp
public string MyMethod()
{
    try
    {
        //several lines of code
    }
    catch
    {
        return "";
    }
}
```
*should be* 🡻

```csharp
public string MyMethod()
{
    try
    {
        //several lines of code
    }
    catch
    {
        Console.WriteLine("some error message");
    }
}
```
