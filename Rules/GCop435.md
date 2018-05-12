# GCop 435

> *"Finally block should not be empty"*

## Rule description

By using a `finally` block, you can clean up any resources that are allocated in a `try` block, and you can run code even if an exception occurs in the `try` block. If the finally statement is empty, it means that you don't need this block at all.

## Example

```csharp
public void MyMethod()
{
    try
    {
        ...
    }
    catch (Exception)
    {
        ...
    }
    finally
    {

    }
}
```

*should be* 🡻

```csharp
public void MyMethod()
{
    try
    {
        ...
    }
    catch (Exception)
    {
        ...
    }
    finally
    {
        //Some code here
    }
}
```
