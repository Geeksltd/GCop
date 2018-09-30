# GCop 128

> *"`IHierarchy / ISortable` interface should be set in M# on the entity definition"*
## Rule description

If you specify the `IHierarchy` or `ISortable` interfaces manually, in the **partial logic class** of an entity, then the M# code generator will not be aware of that.

Instead, you should specify them in your **M# entity definition**, in which case, M# will not only generate the interface implementation code of `... : IHierarchy` or `... : ISortable` on the generated class, but also it will generate **additional code** that is required for a correct implementation of those concepts.