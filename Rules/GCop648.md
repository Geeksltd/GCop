# GCop 648

> *"Use stringPhrase.Remove(oldValue) instead"*

## Rule description

The *Remove()* extension method on string is more readable, more explicit and briefer than using *Replace(..., "")* when you just want to eliminate a phrase.

## Example

```csharp
var clean = textBox.Text.Replace("someText", "");
```

*should be* 🡻

```csharp
var clean = textBox.Text.Remove("someText");
```