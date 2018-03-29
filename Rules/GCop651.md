# GCop651

> *"Wait() method can cause a deadlock. Use the await keyword on the method call instead."*


## Rule description
Wait will synchronously block until the task completes. 
So the current thread is literally blocked waiting for the task to complete. 
As a general rule, you should use "async all the way down"; that is, don't block on async code. 

await will asynchronously wait until the task completes. 
This means the current method is "paused" (its state is captured) and the method returns an incomplete task to its caller. 
Later, when the await expression completes, the remainder of the method is scheduled as a continuation.

## Example 1
```csharp
public static class DeadlockDemo
{
  private static async Task DelayAsync()
  {
    await Task.Delay(1000);
  }
  // This method causes a deadlock when called in a GUI or ASP.NET context.
  public static void Test()
  {
    // Start the delay.
    var delayTask = DelayAsync();
    // Wait for the delay to complete.
    delayTask.Wait();
  }
}
```
*should be* 🡻

```csharp
public static class DeadlockDemo
{
  private static async Task DelayAsync()
  {
    await Task.Delay(1000);
  }
  public static void Test()
  {
    await DelayAsync();
  }
}
```

