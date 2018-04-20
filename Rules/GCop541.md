# GCop 541

> *"Simply the method by removing the `async` and `await` keywords and just return the task directly."*

## Rule description

You do not need to `await` a method which returns a `Task<T>`, the code will just run asynchronously if you have the `async` keyword on the method. The purpose of the `async` keyword is simply to mark a method as being able to return a `Task<T>` and be able to use the await keyword. so if you remove `await` keyword you don’t either need `async` keyword too.

## Example

```csharp
private async Task<bool> MyMethod(int myParam)
{
    return await AnotherAsyncMethod(myParam);
}
```

*should be* 🡻

```csharp
private Task<bool> MyMethod(int myParam)
{
    return AnotherAsyncMethod(myParam);
}
```