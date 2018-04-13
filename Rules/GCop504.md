# GCop 504

> *"The number of arguments in  `{Format / FormatWith}` is incorrect."*
> 
> *"Invalid argument reference in `{Format / FormatWith}`"*

## Rule description

The `string.Format` and `string.FormatWith` methods inject parameters into a string template. The placeholders (such as `{0}` and `{1}`) are meant to correspond to the values passed into the method. If they don't match, it's often a programming mistake that can lead to runtime errors.

## Example1

```csharp
var myVar = string.Format("someText someOtherText", myString);
```

*should be* 🡻

```csharp
var myVar = string.Format("someText {0} someOtherText", myString);
```

## Example2

```csharp
"This is {0} the {1}".FormatWith("not", "greatest", "world");
```

*should be* 🡻

```csharp
"This is {0} the {1}".FormatWith("not", "greatest");
```
