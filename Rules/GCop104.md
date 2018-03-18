# GCop104

> *"Remove empty partial class"*


## Rule description
Blank partial class files are unnecessary and should be removed.
## Example 1
```csharp
public partial class Subset {
}
```
*should be* 🡻

```csharp
 //this class should be removed
```
