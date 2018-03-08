# GCop216

> *"A method named `GetMethodName` is expected return a value. If it's meant to be void, then use a verb other than `Get` such as Read, Download, Sync, ... "*


## Rule description
If a method name starts with Get and then an upper case letter, then if it's void, you should not use the get keyword.Because methods should have names that clearly distinguish their function.


## Example 1
```csharp
public void GetCreditAsync(string Username, string Password) 
{
    //some codes
}
```
*should be* 🡻

```csharp
public void SetCreditAsync(string Username, string Password)
{
    //some codes          
}
```

