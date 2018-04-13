# GCop 504

> *"The number of arguments in  `{Format / FormatWith}` is incorrect."*
> 
> *"Invalid argument reference in `{Format / FormatWith}`"*

## Rule description

we use `string.Format or string.FormatWith` to converts the value of objects to strings based on the formats specified and inserts them into another string.

You should consider about the index of every format item has a matching object in the object list.

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