# GCop 218

> *"For consistency and clarity, use '@this' instead of {'argumentName'} for the first paramter of extension methods"*

## Rule description

The extension method is called like an instance method, but is actually a static method. The instance pointer `this` is a parameter.
You must specify the this-keyword before the appropriate parameter you want the method to be called upon and for clarity it is better to use *@this* as a parameter name.

## Example

```csharp
public static Int64 ToInt64(this string no){...}
```

*should be* 🡻

```csharp
public static Int64 ToInt64(this string @this){...}
```