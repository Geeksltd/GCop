# GCop 547

> *"Remove redundant delegate creation"*

## Rule description

Because the += operator merely concatenates the internal invocation list of one delegate to another, you can use the += to add an anonymous method.

## Example

```csharp
Changed += new EventHandler(OnChanged);
```

*should be* 🡻

```csharp
Changed += OnChanged;
```
