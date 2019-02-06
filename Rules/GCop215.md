# GCop 215

> *"Rename the method to `CountFoo` as it's shorter and more readable"*

## Rule description

It is shorter and more readable to write `CountSomething` as a method name than writing `GetSomethingCount`. 

## Example

```csharp
public int FooCount()
{
    ...
}
```

*should be* 🡻

```csharp
public int CountFoo()
{
    ...
}
```