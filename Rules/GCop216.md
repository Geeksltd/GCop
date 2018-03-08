# GCop216

> *"A method named `GetMethodName` is expected return a value. If it's meant to be void, then use a verb other than `Get` such as Read, Download, Sync, ... "*


## Rule description
If a method name starts with Get followed by an upper case letter, people will expect it to return something. So if it's void, that can be confusing.

There could be cases where the method **gets** something from someone and saves it in the database or file, etc. For example it may be called GetLatestExchangeRate() which invokes a remote Api and saves the result in the database, instead of returning something. In those cases, use a verb other than Get. For example you can call it **ImportLatestExchangeRate()**.


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

