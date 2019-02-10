# GCop 402

> *"Remove unnecessary parenthesis."*

## Rule description

Creating new instances of a class with or without parenthesis are the same. Both will call the default parameter-less constructor.
Adding the parenthesis will be unnecessary noise.

## Example

```csharp
var foo = new Foo() { FisrtProperty = "someText" };
```

*should be* 🡻

```csharp
var foo = new Foo { FisrtProperty = "someText" };
```