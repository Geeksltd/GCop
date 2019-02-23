# GCop 608

> *"A secure file should be made available only to authorized users, not just any user. Apply proper checks."*
>
> *"A secure file should be made available only to authorized users, not just any user. Apply proper checks. Also make the rule more flexible. Sometimes the parameter is not exactly IUser but another type which implements IUser. In that case the rule should still be shown."*

## Rule description

For each secure file type property you must implement a method in the business logic, which is invoked by the M# framework. M# framework requires a specific naming format of `bool Is{PropertyName}VisibleTo(IUser user)` in order to invoke the method.

This method should only require one argument of Interface type `IUser` and must have `bool` return.
You should apply proper checks to restrict access only for authorized users.

## Example1

```csharp
public bool IsMyDocumentVisibleTo(IUser user)
{
    return true;
}
```

*should be* 🡻

```csharp
public bool IsMyDocumentVisibleTo(IUser user)
{
    return user is Administrator;
}
```

*OR* 🡻

```csharp
public bool IsMyDocumentVisibleTo(IUser user)
{
    return user is Customer customer && customer.Organisation == this.Organisation ;
}
```

## Example2

```csharp
public bool IsMyDocumentVisibleTo(IUser user)
{
    return user != null; // This is not a real security check because 'user' is never null, even for anonymous users.
}
```

*should be* 🡻

```csharp
public bool IsMyDocumentVisibleTo(IUser user)
{
    return user is Administrator;
}
```
*or some other code with real security checking logic*
