# GCop 128

> *"\{IHierarchy} interface should be set in M# on the entity definition"*
>
> *"\{ISortable} interface should be set in M# on the entity definition"*

## Rule description

The `IHierarchy` interface is available under `MSharp.Framework.Services` namespace, which contains definitions of two methods to implement parent and children hierarchies and one property to define the name of the current entity instance in hierarchal data. So it should be implemented on the entity definition.

The `ISortable` interface is defined under "MSharp.Framework.Services" and contains only one int type property definition. This property must be implemented in the entity type in order to perform sorting on the collection of an entity instance.

## Example

```csharp
namespace Domain
{
    public class MyClass: IHierarchy
    {
        ...
    }
}
```

*should be* 🡻

```csharp
namespace Domain
{
    public class MyEntityTypeClass: IHierarchy
    {
        ...
    }
}
```