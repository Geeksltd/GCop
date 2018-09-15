# GCop 185

> *"Call `ConfigureAwait(false)`"*

## Rule description

`ConfigureAwait(false)` configures the task so that continuation after the `await` does not have to be run in the caller context, therefore avoiding any possible deadlocks.

## Example

```csharp
public async Task SomeAsync()
{
    await GetValueAsync();
}
```

*should be* 🡻

```csharp
public async Task SomeAsync()
{
    await GetValueAsync().ConfigureAwait(false);
}
```
