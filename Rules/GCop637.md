# GCop 637

> *"This will be invoked twice: once for a clone of the object, and once for the actual object.
The method result may be different on the second call, leading to inconsistency.
Instead save the result of the method call in a local variable before Database.Update() call and set [\{PropertyName}] to that variable."*

## Example

```csharp
public void Activate()
{
    Database.Update(this, o => o.Email = CallAnotherMethod());
}
```

*should be* 🡻

```csharp
public void Activate()
{
    var myEmail = CallAnotherMethod();
    Database.Update(this, o => o.Email = myEmail);
}
```
