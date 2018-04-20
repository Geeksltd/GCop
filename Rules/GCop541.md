# GCop 541

> *"Simply the method by removing the `async` and `await` keywords and just return the task directly."*

## Rule description

The purpose of the `async` keyword is only to mark a method as being able to use the `await` keyword.
A method that returns a `Task` or `Task<T>` doesn't necessarily need to be marked as `async`.
 
If all your `async` method is doing is to `await` a task, then you can drop both `async` and `await` and let the method simply return the task back to its caller. This will not only be cleaner, but also faster to run.

## Example

```csharp
async Task<bool> MyMethod(int myParam)
{
    return await AnotherAsyncMethodOrExpression(myParam);
}
```

*should be* 🡻

```csharp
Task<bool> MyMethod(int myParam)
{
    return AnotherAsyncMethodOrExpression(myParam);
}
```
