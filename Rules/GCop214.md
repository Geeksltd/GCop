# GCop 214

> *"The variable defined to return the result of the method should be named `result`"*

## Rule description

When someone is reading the code of a method, every time they see a variable declaration, they have to think about the role or purpose of it in order to understand it.

Often you need to define a variable that is used to hold the return value of the method. In those cases, use the convention to name it "result", so the reader will immediately understand its purpose.

## Example

```csharp
public string Foo()
{
    var bar = "something";
    ...
    return bar;
}
```

*should be* 🡻

```csharp
public string GetTheThing()
{
    var result = "something";
    ...
    return result;
}
```
