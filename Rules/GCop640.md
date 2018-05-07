# GCop 640

> *"Write it as \{CollectionName}.Contains(VALUE)"*
>
> *"Write it as \{CollectionName}.Except(VALUE).Any() "*

## Rule description

The `Contains()` method determines whether an element is in the List. It is an instance method which takes an object, while `Any()` is an extension method which takes a predicate. So if you want to check for a specific condition, use `Any`. If you want to check for the existence of an element, use `Contains`.

## Example1

```csharp
var myResult = myCollection.Any(a => a == someValue);
```

*should be* 🡻

```csharp
var myResult = myCollection.Contains(someValue);
```

## Example2

```csharp
var myResult = myCollection.Any(a => a != someValue);
```

*should be* 🡻

```csharp
var myResult = myCollection.Except(someValue).Any();
```