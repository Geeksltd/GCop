# GCop 210

> *"Suffix the name of a service class with `Service` as it's inside the Services folder."*

## Rule description

Using "*Service*" as a suffix for a class indicates that the class represents a service, which is assumed to be remote.

## Example

```csharp
namespace Foo.Service
{
    public class Bar 
    {   
        ...   
    }
}
```

*should be* 🡻

```csharp
namespace Foo.Service
{
    public class BarService 
    {   
        ...   
    }
}
```