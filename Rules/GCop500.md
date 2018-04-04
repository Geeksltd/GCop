# GCop500

> *"Void async methods cannot be awaited. Also they can hide exceptions and cause tricky bugs. Return a Task instead."*


## Rule description
Asynchronous methods report exceptions through the Task object. When an exception is thrown, the Task enters the faulted state. When you await a faulted task, the await expression throws the exception. When you await a task that faults later, the exception is thrown when the method is scheduled to resume.

In contrast, async void methods cannot be awaited. There’s no way for the code that calls an async void method to catch or propagate an exception thrown. Don’t write async void methods because errors are hidden from callers.

### What if you have no choice?
There are cases when you cannot change the method to return a Task. For example when the method is an event handler that has to conform to a pre-defined signature that you cannot change. Or when it's overriding a method and cannot change the signature. In such cases you must at least handle exceptions within the async method itself and log it or show it in the debug console to at least make it possilble to diagnose the problem later.

Ideally during development, you should break the debugger at that point to make it possible to find the error.

## Example 1
```csharp
public async void MyMethod()
{
    ...
    await SomethingThatMayThrow();
}
```
*should be* 🡻

```csharp
public async Task MyMethod()
{
    ...
    await SomethingThatMayThrow();
}
```
*Or, if it's not possible, at least* 🡻
```csharp
static async void MyMethod()
{
  try { await SomethingThatMayThrow(); }
  catch (Exception ex) 
  {
      Log("Exception handled OK");
      System.Diagnostics.Debugger.Break(); 
  }
}
```
