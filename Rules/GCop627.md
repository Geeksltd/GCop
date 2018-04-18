# GCop 627

> *"Instead of « ?? false » use the more readable expression of « == true »"*
> 
> *"Instead of !(nullable expression == true) use the more readable alternative of: (nullable expression == false)."*

## Rule description

Human brain can understand positive phrases better than negative ones so it is recommended to use `==false` rather than `!nullable expression == true` to have a more readable code.

Also using `??false` is like using `==true` but is less meaningful.understand 

## Example1

```csharp
bool? nullableBool = true;

if(nullableBool ?? false){...}
```

*should be* 🡻

```csharp
bool? nullableBool = true;

if(nullableBool == true){...}
```

## Example2

```csharp
if(!boolVar == true){...}
```

*should be* 🡻

```csharp
if(boolVar == false){...}
```
