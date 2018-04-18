# GCop 639

> *"It should be written as if ({'yourObject'}?.{'Property'} != null)"*

## Rule description

The `?.` operator lets you access members and elements only when the receiver is not-null, returning null result otherwise. It is exactly what we except when checking an object is null or not like the samples below, while `?.` is more meaningful and readable.

## Example

```csharp
if (myCookie != null && myCookie.Value != null)
{
    ...
}
```

*should be* 🡻

```csharp
if (myCookie?.Value != null)
{
    ...
}
```