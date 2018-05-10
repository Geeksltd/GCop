# GCop 608

> *"A secure file should be made available only to authorised users, not just any user. Apply proper checks."*
>
> *"A secure file should be made available only to authorised users, not just any user. Apply proper checks. Also make the rule more flexible. Sometimes the parameter is not exactly IUser but another type which implements IUser. In that case the rule should still be shown."*

## Rule description

For each secure file type property you must implement a method in the business logic, which is invoked by the M# framework. M# framework requires a specific naming format “Is + PropertyName + VisibleTo” in order to invoke the method. This method should only require one argument of Interface type `IUser` and must have `bool` return.

You should apply proper checks to restrict access only for authorised users.

## Example1

```csharp
public bool IsUserVisibleTo(IUser user)
{
    return true;
}
```

*should be* 🡻

```csharp
public bool IsUserVisibleTo(IUser user)
{
    if (user is Administrator)
        return true;
    return false;
}
```

## Example2

```csharp
public bool IsUserVisibleTo(IUser user)
{
    return user != null;
}
```

*should be* 🡻

```csharp
public bool IsUserVisibleTo(IUser user)
{
    return user != null && user is Administrator;
}
```