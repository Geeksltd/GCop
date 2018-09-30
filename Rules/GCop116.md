# GCop 116

> *"Break this down into smaller methods. If such methods would become meaningless as standalone methods in the context of the class, you can refactor this method into a Stateful Service class"*

## Rule description

The first rule of functions is that they should be small. The second rule of functions is that they should be smaller than that.

Functions should do one thing. They should do it well. They should do it only, so long methods are against OOP rules and reduce the code readability.

## Example

```csharp
public bool Foo()
{
    //more than 40 statements
}
```

*should be* 🡻

```csharp
public bool Foo()
{
    //Call other methods
    //less than 40 statements
}
//Smaller methods
```