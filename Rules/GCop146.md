# GCop 146

> *"This public method doesn't seem to be used in the solution. Consider removing it."*
> 
> *"Consider making it `internal` as it's not used outside of this project."*
> 
> *"Consider making it `private` as it's not used outside of this class."*

## Rule description

To better manage complexity, a class member (method, property, ...) should be as private as possible, and only promoted to internal or public knowingly and when required.

Private methods help prevent people from depending on certain parts of your code. For example, say you want to implement some data structure. You want users of your data structure to not care how you implemented it, but rather just use the implementation through your well defined interface. If no one is depending on your implementation, you can change it whenever you want without effecting the class users. This is a great benefit that using private methods (and more broadly, encapsulation) gives you.

### GCop's assumption
GCop assumes that your code is meant to be used within the same solution only. This is the case with most enterprise application classes. If, however, your project is meant to be used as a library outside of the same solution, then you should disable this rule.

## Example

```csharp
public void Foo()
{
     ...
}
```

*should be* 🡻

```csharp
private void Foo()
{
     ...
}
```

*OR* 🡻

```csharp
internal void Foo()
{
     ...
}
```

*OR* 🡻

```csharp
//Remove the unused method
```