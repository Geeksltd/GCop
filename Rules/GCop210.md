# GCop210

> *"Suffix name of a service class with Service"*


## Rule description
using *Service* as a suffix for a class indicates that the class represents a service, which is assumed to be remote.

## Example 1
```csharp
public static Port CreatePort(string PortName, string BindingName, string targetNamespace)
{
    ...
}
```
*should be* 🡻

```csharp
(...corrected version)
```
