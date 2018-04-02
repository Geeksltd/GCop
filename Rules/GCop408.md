# GCop408

> *"Flag or switch parameters (bool) should go after all non-optional parameters. If the boolean parameter is not a flag or switch, split the method into two different methods, each doing one thing."*


## Rule description
If the purpose of a boolean argument is a flag or a switch, it should go after the main parameters for better readability, and also to enable the caller to specify the name of the parameter before the *true/faalse* expression [see GCop117](GCop117.md).

If the role of the boolean argument is not just a switch, and instead it's to specify the primary action, then the method design should change into two separate methods.

## Example 1
```csharp
public static void Login(bool rememberPassword, string userId)
{
    ...
}
```
*should be* 🡻

```csharp
public static void Login(string userId, bool rememberPassword)
{
    ...
}
```

## Example 2
```csharp
public void Invite(bool telephone, User user)
{
    if (telephone)
    {
        // ... Send an invitation SMS
    }
    else 
    {
        // ... Send an invitation email
    }
}
```
*should be* 🡻

```csharp
public void InviteByTelephone(User user)
{
     // ... Send an invitation SMS
}

public void InviteByEmail(User user)
{
     // ... Send an invitation email
}
```

