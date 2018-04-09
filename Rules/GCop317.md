# GCop 317

> *"This code is repeated 'repeatation count' times in this method. If its value remains the same during the method execution, store it in a variable. Otherwise define a method (or Func<T> variable) instead of repeating the expression. 'expression type'"*

## Rule description

We should write as short and simple code as it is possible. Repeatation will reduce the clarity of the code.

## Example 1

CastExpression

```csharp
public void MyMethod()
{
    if(something == otherthing)
    {
        AnotherMethod((int)Status.Wait, "thisString");
    }
    else 
    {
        AnotherMethod((int)Status.Wait, "anotherString");
    }
}
```

*should be* 🡻

```csharp
public void MyMethod()
{
    int waitStatus = (int)Status.Wait;
    
    if(something == otherthing)
    {
        AnotherMethod(waitStatus, "thisString");
    }
    else 
    {
        AnotherMethod(waitStatus, "anotherString");
    }
}
```

## Example 2

ReturnStatement

```csharp
public ServerResult MyMethod()
{
    if(something == otherthing) return new ServerResult { Success = false, Message = "sometext" };
    try
    {
        ...
    } 
    catch
    {
        return new ServerResult { Success = false, Message = "sometext" };
    }
}
```

*should be* 🡻

```csharp
public ServerResult MyMethod()
{
    if(something == otherthing) return CreateError();
    
    try
    {
        ...
    } 
    catch
    {
        return CreateError();
    }
}

ServerResult CreateError() => new ServerResult { Success = false, Message = "sometext" };
```

*OR it can even be an inline method inside the parent method* 🡻

```csharp
public ServerResult MyMethod()
{
    ServerResult error() => new ServerResult { Success = false, Message = "sometext" };
    
    if(something == otherthing) return error();
    
    try
    {
        ...
    } 
    catch
    {
        return error();
    }
}
```