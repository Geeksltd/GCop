# GCop 218

> *"For consistency and clarity, use '@this' instead of {'argumentName'} for the first paramter of extension methods"*

## Rule description

Extension methods are meant to be called like an instance method. The first parameter needs the `this` keyword also to mark the method as an Extension method.

To further clarify that the role of this parameter is indeed to specify the `this` instance, a good practice is to always name it as `@this` so that the purpose and context of the implementation is more obvious.

## Example

```csharp
public static Int64 ToInt64(this string no){...}
```

*should be* 🡻

```csharp
public static Int64 ToInt64(this string @this){...}
```
