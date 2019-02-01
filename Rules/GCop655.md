# GCop 655

> *"Change this method to return a `Task`"*

## Rule description

`async void` methods have different error-handling semantics. When an exception is thrown out of an async `Task` or async `Task<T>` method, that exception is captured and placed on the `Task` object. With `async void` methods, there is no `Task` object, so any exceptions thrown out of an `async void` method will be raised directly on the SynchronizationContext that was active when the `async void` method started. 

Async methods returning `void` don’t provide an easy way to notify the calling code that they’ve completed. It’s easy to start `async void` methods, but it’s not easy to determine when they’ve finished. 

It’s clear that `async void` methods have several disadvantages compared to `async Task` methods, but they’re inevitable in a particular case: asynchronous event handlers. [Further reading](https://msdn.microsoft.com/en-us/magazine/jj991977.aspx).

## Example

```csharp
public async void Foo()
{
    await Bar();
}
public async Task Bar()
{
    await Task.Delay(1000);
}
```

*should be* 🡻

```csharp
public async Task Foo()
{
    await Bar();
}
public async Task Bar()
{
    await Task.Delay(1000);
}
```
