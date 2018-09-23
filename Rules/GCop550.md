# GCop 550

> *"Return Task.FromResult instead of returning null"*

## Rule description

The `Task.FromResult<object>(null)` method is commonly used to generate a task whose result is null. We should never return a null Task. In the async world, a null task just doesn't make sense. Task represents the execution of the asynchronous method, so for an asynchronous method to return a null task is like telling the calling code "you didn't really just call this method" when of course it did.
So, a `Task`/`Task<T>` returned from a method should never, ever be null. However, you still have the option of returning a null value inside a regular task.

## Example1

```csharp
Task<object> GetAsync()
{
    return null;
}
```

*should be* 🡻

```csharp
Task<object> GetAsync()
{
    return Task.FromResult<object>(null);
}
```

## Example2

```csharp
Task<object> GetAsync()
{
    return _someThing?.GetAsync(); // RCS1210
}
```

*should be* 🡻

```csharp
Task<object> GetAsync()
{
    SomeThing x = _someThing;
    if (x != null)
    {
        return _someThing.GetAsync();
    }
    else
    {
        return Task.FromResult<object>(null);
    }
}
```

