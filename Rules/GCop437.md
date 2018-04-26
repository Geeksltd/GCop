# GCop 437

> *"Complete the task associated to this \"TODO\" comment."*

## Rule description

When in development, many tasks need remembering. We track them in Visual Studio with `TODO` comments. These comments help organize our projects. We list to-do items in a central place.

## Example

```csharp
//TODO Create method
```

*should be* 🡻

```csharp
//Method created
public void MyMethod()
{
    ...
}
```