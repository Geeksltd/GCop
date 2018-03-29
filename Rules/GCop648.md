# GCop648

> *"Use stringPhrase.Remove(oldValue) instead"*


## Rule description
...

## Example 1
```csharp
textBoxText.Replace("someText", "");
```
*should be* 🡻

```csharp
textBoxText.Remove("someText");
```

