# GCop 140

> *"Consider making it "private" as it's not used outside of this class."*
> 
> *"This private method doesn't seem to be used in the solution. Consider removing it."*
> 
> *"Consider making it "internal" as it's not used outside of this project."*

## Rule description

To better manage complexity, a class member (method, property, ...) should be as private as possible, and only promoted to internal or public knowingly and when required.

Private variables help prevent people from depending on certain parts of your code. For example, say you want to implement some data structure. You want users of your data structure to not care how you implemented it, but rather just use the implementation through your well defined interface.

If no one is depending on your implementation, you can change it whenever you want without effecting the class users. This is a great benefit that using private variables (and more broadly, encapsulation) gives you.

## Example

```csharp
public static void MyMethod() {...}
```

*should be* 🡻

```csharp
internal static void MyMethod(){...}
```
