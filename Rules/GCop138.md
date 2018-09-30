# GCop 138

> *"When you catch an exception you should throw exception or at least log the error"*

## Rule description

If you catch an exception and just silence the error, it can potentially lead to hard-to-find bugs.

Generally you should either:
- Think about exception scenarios carefully and handle each type of exception specifically.
- Let the exception be thrown and cascaded so that someone higher up in the call chain can handle it.

There may be scenarios that just don't want the method to throw an exception and would rather silence the error. Those cases are rare, but it happens. In those cases you must at least:

- Log the error, so it can be investigated later.
- Write a message to Console, so at least during development and debugging it has a chance of being noticed.
- If you believe that neither of the above are necessary in your case, document that explicitly by adding a comment such as "No logging is needed".

If the body of an exception block doesn't re-throw the error, Gcop will look for the term `log` or `Console` in your code. It doesn't care if it's a real log invocation or a comment. The purpose here is to ensure you have thought about this and made a conscious decision, rather than omitting it by accident or ignorance.


## Keeping information about the original exception
When you catch an exception and then throw a new exception, the stack trace (containing information about the original exception) will get lost. To prevent that you should either: 
- re-throw the original exception (by using the `throw;` command with no parameters) or
- pass the original exception as the "InnerException" of the new one.

## Example 1

```csharp
public string Foo()
{
    try
    {
        ...
    }
    catch
    {
        return "";
    }
}
```

*should be either* 🡻

```csharp
public string Foo()
{
    try
    {
        ...
    }
    catch
    {
        // some logic...
        throw;
    }
}
```

*OR* 🡻

```csharp
public string Foo()
{
    try
    {
        ...
    }
    catch (Exception ex)
    {
        throw new Exception("Some useful error message related to Foo", ex);
    }
}
```

*OR* 🡻

```csharp
public string Foo()
{
    try
    {
        ...
    }
    catch (Exception ex)
    {
        Log.Error(ex);
        return "";
    }
}
```

*OR* 🡻

```csharp
public string Foo()
{
    try
    {
        ...
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR in Foo: " + ex.Message);
        return "";
    }
}
```

*OR* 🡻

```csharp
public string Foo()
{
    try
    {
        ...
    }
    catch (Exception ex)
    {
        // No logging is needed because ...
        return "";
    }
}
```

## Example 2

```csharp
public string Foo()
{
    try
    {
        ...
    }
    catch
    {
        throw new Exception("Some error message");
    }
}
```

*should be* 🡻

```csharp
public string Foo()
{
    try
    {
        ...
    }
    catch (Exception ex)
    {
        throw new Exception("Some error message", ex);
    }
}
```