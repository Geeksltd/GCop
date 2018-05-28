# GCop 515

> *"Mark \{PropertyName} in M# as Unique. It will then generate the FindBy\{PropertyName} method for you."*

## Rule description
> see https://github.com/Geeksltd/GCop/issues/165

M# generates a method for each property which is marked as unique, to find a record in the database based on the property value. This method start with "FindBy" keyword. A uniqueness validation is also applied on the property marked as unique in the “ValidateProperties” method of the entity class. So there is no need to implement all of these by yourself.

## Example

```csharp
//String("PhoneNumber", 11).Mandatory()
public static Task<User> FindByPhoneNumber(string phoneNumber)
{
    return Database.FirstOrDefault<User>(u => u.PhoneNumber == phoneNumber);
}
```

*should be* 🡻

```csharp
//String("PhoneNumber", 11).Mandatory().Unique();
public static Task<User> FindByPhoneNumber(string phoneNumber)
{
    return Database.FirstOrDefault<User>(u => u.PhoneNumber == phoneNumber);
}
```
