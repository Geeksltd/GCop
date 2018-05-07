# GCop 543

> *"The Result property of the Task object should never be called. Either properly await it, or call GetAlreadyCompletedResult()."*

## Rule description

Calling the *`Result`* property will synchronously block until the task completes. So the current thread is literally blocked waiting for the task to complete. That [can cause deadlocks](http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html) and other problems depending on the implementation of that task.

### Default correct way
As a general rule, you should use "[async all the way down](https://stackoverflow.com/questions/29808915/why-use-async-await-all-the-way-down)" to avoid blocking code. Make sure to learn [async programming best practices](https://msdn.microsoft.com/en-us/magazine/jj991977.aspx?f=255&MSPPError=-2147217396).

With *`await`* your code will asynchronously wait until the task completes. This means the current method is "paused" (its state is captured) and the method returns an incomplete task to its caller. Later, when the await expression completes, the remainder of the method is scheduled as a continuation.

### Special scenario A
The `.GetAlreadyCompletedResult()` method should be used when you know that the task is already completed, and you just need to unwrap the result out of the Task object.

The benefit of this method compared to calling `.Result` is that if you've made a mistake, and the task is not indeed already completed, rather than blocking the thread or risking a deadlock, it will throw an exception, which helps you debug your assumptions.

### Special scenario B
There might be cases where you have no choice but to syncrhronously wait for the task to be completed. This is rare, but may sometimes be inevitable. In such cases, you should call `.RiskDeadlockAndAwaitResult()` rather than simply calling `.Result` which is an explicit way of confirming that you know what you are doing.

## Example

```csharp
public static class DeadlockDemo
{
   static async Task<Something> GetSomething()
   {
      ...
   }
  
  public static void Test()
  {
      // This method can causes a deadlock when called in a GUI or ASP.NET context.
      var something = GetSomething().Result;
      
      // .... (use something)
  }
}
```

*should be* 🡻

```csharp
public static class DeadlockDemo
{
   static async Task<Something> GetSomething()
   {
      ...
   }
  
  public static async Task Test()
  {
      var something = await GetSomething();
      
      // .... (use something)
  }
}
```

*OR* 🡻

```csharp
public static class DeadlockDemo
{
   static async Task<Something> GetSomething()
   {
      ...
   }
  
  public static void Test()
  {
      // Use only if you know for sure that the task is completed,
      // and also your method HAS TO be a void rather than a Task
      var something = GetSomething().GetAlreadyCompletedResult(); 
      // .... (use something)
  }
}
```

*OR* 🡻

```csharp
public static class DeadlockDemo
{
   static async Task<Something> GetSomething()
   {
      ...
   }
  
  public static void Test()
  {
      // Use if your method HAS TO be a void rather than a Task,
      // and know what you're doing.
      var something = GetSomething().RiskDeadlockAndAwaitResult(); 
      // .... (use something)
  }
}
```
