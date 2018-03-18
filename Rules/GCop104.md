# GCop104

> *"Remove empty partial class"*


## Rule description
The empty partials should not be overwritten if they already exist (they're meant to be customized by developers).

## Example 1
```csharp
public partial class Subset {
}
```
*should be* 🡻

```csharp
 //this class should be removed
```
