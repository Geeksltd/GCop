# GCop 547

> *"Remove redundant delegate creation"*

## Rule description

The `[object].[event] += anEvent;` phrase is just a syntactic sugar for `[object].[event] += new EventHandler(anEvent);` and there is no difference between these two. For the first phrase, the compiler will automatically infer the delegate you would like to instantiate. In the second example, you explicitly define the delegate.
## Example

```csharp
changed += new EventHandler(OnChanged);
```

*should be* 🡻

```csharp
changed += OnChanged;
```
