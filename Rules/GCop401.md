# GCop 401

> *"Instead of setting the properties in separate lines, use constructor initializers."*

## Rule description

Object initializers let you assign values to any accessible fields or properties of an object at creation time without having to invoke a constructor followed by lines of assignment statements.

## Example

```csharp
var foo = new Foo();
foo.Property1 = "somethings";
foo.Property2 = 2;
foo.Property3 = 1.2;
```

*should be* 🡻

```csharp
var foo = new Foo
{
    Property1 = "somethings";
    Property2 = 2;
    Property3 = 1.2;
};
```