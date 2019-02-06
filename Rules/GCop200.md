# GCop 200

> *"Since the class is an attribute, the name of the class must end with 'Attribute'"*

## Rule description

An attribute is a class that is derived from the `Attribute` class through inheritance. We specify to use the attribute with square brackets and the name of the attribute, omitting the word "Attribute" on the end for short syntax like `[SampleAttribute]` or `[Sample]`. So an attribute class name should end with Attribute - 'A' being capital.

## Example1

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class Foo : Attribute
{
    ...
}
```

*should be* 🡻

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class FooAttribute : Attribute
{
    ...
}
```

## Example2

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class Fooattribute : Attribute
{
    ...
}
```

*should be* 🡻

```csharp
[AttributeUsage(AttributeTargets.Class)]
public class FooAttribute : Attribute
{
    ...
}
```