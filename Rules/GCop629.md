# GCop 629

> *"\{EntityName} already has a property named \{PropertyName}, Use that instead of ID"*

## Rule description

> This seems wrong. See https://github.com/Geeksltd/GCop/issues/150

Each data row should be unique in the system and be accessed with a reference.
When you create a new Entity, M# generates by default an ID property, which is a Guid and not visible in the M# Entity editor. 

 If each instance can have a unique reference number, which will never change, this property could be used instead of the ID property.

## Example

```csharp
var myCustomer = new Customer
{
    ID = SomeGuidValue,
    ...
};
```

*should be* 🡻

```csharp
var myCustomer = new Customer
{
    CustomerId = SomeValue,
    ...
};
```
