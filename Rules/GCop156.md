# GCop 156

> *"Should be written as CookieProperty.Remove(\{key})"*

## Rule description

To remove a cookie, rather than setting the value to null, the `Remove()` method should be used. Otherwise the cookie will still remain on the browser (albeit with the value of null).

## Example

```csharp
CookieProperty.Set("mykey", null);
```

*should be* 🡻

```csharp
CookieProperty.Remove("mykey");
```
