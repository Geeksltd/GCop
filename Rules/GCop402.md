# GCop 402

> *"Remove unnecessary parenthesis."*

## Rule description

Creating new instances of a class with or without paranthesis are the same. Both will call the default parameter-less constructor.
Adding the paranthesis will be unnecessary noise.

## Example

```csharp
var myObject = new MyClassName() { FisrtProperty = "someText" };
```

*should be* 🡻

```csharp
var myObject = new MyClassName { FisrtProperty = "someText" };
```