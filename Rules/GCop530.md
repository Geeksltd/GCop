# GCop 530

> *"Write this as \{variableName}.WithSuffix(\{SuffixString})."*

## Rule description

The `WithSuffix("SuffixString")` Returns your string with the specified suffix if it has a value. If your string is empty or null, it will return empty string. As you can see in the below example it is easy to use and more readable rather than conditional operator.

## Example

```csharp
var result = myStringObj.HasValue() ? myStringObj + "something" : string.Empty;
```

*should be* 🡻

```csharp
var result = myStringObj.WithSuffix("something");
```