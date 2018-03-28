# GCop500
> *"Method should not be void"*


## Rule description
Asynchronous methods report exceptions through the Task object. When an exception is thrown, the Task enters the faulted state. When you await a faulted task, the await expression throws the exception. When you await a task that faults later, the exception is thrown when the method is scheduled to resume.

In contrast, async void methods cannot be awaited. There’s no way to for the code that calls an async void method to catch or propagate an exception thrown from the async method. Don’t write async void methods because errors are hidden from callers.

You can handle exceptions within the async method itself. You can log it, and possibly save data, but you cannot prevent the uncaught exception from terminating the application.

## Example 1
```csharp
public async void MyMethod()
{
    await ThrowAsync();
}
```
*should be* 🡻

```csharp
public async Task MyMethod()
{
    await ThrowAsync();
}
```
*OR* 🡻
```csharp
static async void MyMethod()
{
  try { await ThrowAsync(); }
  catch (Exception ex) { Log("Exception handled OK"); }
}
```
