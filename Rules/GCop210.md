# GCop210

> *"Suffix the name of a service class with 'Service' as it's inside the Services folder."*


## Rule description
using *Service* as a suffix for a class indicates that the class represents a service, which is assumed to be remote.

## Example 1
```csharp
namespace MyNameSpace.Service
{
    public class SampleClass 
    {   
        ...   
    }
}
```
*should be* 🡻

```csharp
namespace MyNameSpace.Service
{
    public class SampleClassService 
    {   
        ...   
    }
}
```
