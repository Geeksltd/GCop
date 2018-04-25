# GCop 531

> *"Write this as \{variableName}.WithPrefix(\{PrefixString})."*

## Rule description

The `WithPrefix("PrefixString")` Returns your string with the specified prefix if it has a value. If your string is empty or null, it will return empty string. As you can see in the below example it is easy to use and more readable rather than conditional operator.

## Example

```csharp
var result = myStringObj.HasValue() ? "something" + myStringObj : string.Empty;
```

*should be* 🡻

```csharp
var result = myStringObj.WithPrefix("something");
```