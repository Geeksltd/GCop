# GCop 218

> *"For consistency and clarity, use '@this' instead of {'argumentName'} for the first parameter of extension methods"*

## Rule description

Extension methods are meant to be called like an instance method. The first parameter needs the `this` keyword also to mark the method as an Extension method.

To further clarify that the role of this parameter is indeed to specify the `this` instance, a good practice is to always name it as `@this` so that the purpose and context of the implementation is more obvious.

## Example

```csharp
public static int CountUniqueWords(this string something)
{
    ...
}
```

*should be* 🡻

```csharp
public static int CountUniqueWords(this string @this)
{
    ...
}
```
