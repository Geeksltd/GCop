# GCop 156

> *"Should be written as CookieProperty.Remove(\{key})"*

## Rule description

The `CookieProperty.Set()` method sets a specified value in the response cookie as well as request cookie. Set a null value as key is equivalent to use `CookieProperty.Remove()` method, while the second one is more readable and meaningful.
## Example

```csharp
CookieProperty.Set<string>(null);
```

*should be* 🡻

```csharp
CookieProperty.Remove<string>();
```