# GCop 104

> *"Remove empty partial class"*

## Rule description

Blank `partial` class files are unnecessary and should be removed.

## Example

```csharp
public partial class Foo {
}
```

*should be* 🡻

```csharp
 //this class should be removed
```