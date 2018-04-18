# GCop 517

> *"{'MethodName()'} returns a value but doesn't change the object. It's meaningless to call it without using the returned result."*

## Rule description

When you invoke a method on an immutable object such as `string`, `DateTime`, etc, it won't change the state of that object. For example the `AddDays()` method of a `DateTime` object will return a new `DateTime` object and doesn't change the original instance. Therefore it's meaningless to invoke it without using the returned value.

## Example

```csharp
...
CurrentDateTime.AddHours(hours);
...
```

*should be* 🡻

```csharp
...
var something = CurrentDateTime.AddHours(hours);
...
```
*OR* 🡻

```csharp
...
return CurrentDateTime.AddHours(hours);
...
```
