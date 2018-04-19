# GCop 643

> *"Change to return {'xVariable'} ?? {'yVariable'};"*
> 
> *"Change to return {'xVariable.GetIdentifier()'} ?? {'yVariable'};"*
> 
> *"Change to {'result'} = {'xVariable'} ?? {'yVariable'};"*

## Rule description

`if` is great for flow-control, but if you are just returning one of two values or assigning one of two values based on a condition, it is better to use `??` as appropriate.

## Example1

```csharp
if (myBoolObj != null)
    return myBoolObj;
else 
    return anotherBoolObj;
```

*should be* 🡻

```csharp
return myBoolObj ?? anotherBoolObj;
```

## Example2

```csharp
if (myBoolObj != null)
    result = myBoolObj;
else result = null;
```

*should be* 🡻

```csharp
result = myBoolObj ?? result;
```