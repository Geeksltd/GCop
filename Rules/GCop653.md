# GCop 653

> *"Use the `await` keyword on the method call instead of `Foo().Result;` Also it can be used `Foo().GetAwaiter().GetResult();` for better exception handling."*

## Rule description

`Result` will synchronously block until the task completes. So the current thread is literally blocked waiting for the task to complete. That [can cause deadlocks](http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html) and other problems.

As a general rule, you should use "[async all the way down](https://stackoverflow.com/questions/29808915/why-use-async-await-all-the-way-down)" to avoid blocking code. Make sure to learn [async programming best practices](https://msdn.microsoft.com/en-us/magazine/jj991977.aspx?f=255&MSPPError=-2147217396).

With `await` your code will asynchronously wait until the task completes. This means the current method is "paused" (its state is captured) and the method returns an incomplete task to its caller. Later, when the await expression completes, the remainder of the method is scheduled as a continuation.

`GetAwaiter.GetResult()` will just throw the exception caused directly, while `Task.Result` will throw an Aggregate Exception. This tends to make exception stack traces a lot more useful while using `GetAwaiter.GetResult()`.

## Example

```csharp
public static class DeadlockDemo
{
  static async Task<int> Foo()
  {
    ...
  }
  // This method causes a deadlock when called in a GUI or ASP.NET context.
  public static void Bar()
  {
    // Wait for the delay to complete.
    var res = Foo().Result;
  }
}
```

*should be* 🡻

```csharp
public static class DeadlockDemo
{
  static async Task<int> Foo()
  {
    ...
  }
  public static void Bar()
  {
    var res = await Foo();
  }
}
```

*OR* 🡻

```csharp
public static class DeadlockDemo
{
  static async Task<int> Foo()
  {
    ...
  }
  public static void Bar()
  {
    var res = Foo().GetAwaiter().GetResult();
  }
}
```

