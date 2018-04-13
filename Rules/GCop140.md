# GCop 140

> *"Consider making it "private" as it's not used outside of this class."*
> 
> *"This private method doesn't seem to be used in the solution. Consider removing it."*
> 
> *"Consider making it "internal" as it's not used outside of this project."*

## Rule description

Methods are declared in a class or struct by specifying an optional access level, such as public, internal or private. The default is private.

Unused methods reduce code readability and it’s better to prevent from this.

## Example

```csharp
public static void MyMethod(){...}
```

*should be* 🡻

```csharp
private static void MyMethod(){...}
```
