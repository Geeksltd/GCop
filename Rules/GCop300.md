# GCop300

> *"Replace with None()"*
>
> *"Replace with IsSingle()"*
> 
> *"Replace with Any()"*


## Rule description
It's better to use *None()* , *IsSingle()* or *Any* in some scenarios, instead of using *Count()*.

- Faster: Executing a count operation can be time consuming sometimes and involved complex operations. If you only care if there is AT LEAST ONE item, why run the calculation on every item in the list?
 - More readable and expressive: For example *Any()* is more to-the-point and expressive of your intent, rather than comparing Count() against 1.

## Example 1
```csharp
if(myIEnumerableCollection.Count() == 0)
{
    ...
}
```
*should be* 🡻

```csharp
if(myIEnumerableCollection.None())
{
    ...
}
```

## Example 2
```csharp
if(myIEnumerableCollection.Count() == 1)
{
    ...
}
```
*should be* 🡻

```csharp
if(myIEnumerableCollection.IsSingle())
{
    ...
}
```

## Example 3
```csharp
if(myIEnumerableCollection.Count() > 0)
{
    ...
}
```
*should be* 🡻

```csharp
if(myIEnumerableCollection.Any())
{
    ...
}
```

## Example 4
```csharp
if(myIEnumerableCollection.Count() >= 1)
{
    ...
}
```
*should be* 🡻

```csharp
if(myIEnumerableCollection.Any())
{
    ...
}
```
