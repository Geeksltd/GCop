# GCop402

> *"Remove unnecessary parenthesis."*


## Rule description
Creating new instances of a class with or without paranthesis have no difference. Both will call the default parameter-less constructor.
 So using paranthesis just increase code length.

## Example 1
```csharp
var myObject = new MyClassName() { FisrtProperty = "someText" };
```
*should be* 🡻

```csharp
var myObject = new MyClassName { FisrtProperty = "someText" };
```

