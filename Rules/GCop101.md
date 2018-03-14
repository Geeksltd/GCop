# GCop101

> *"Too many parameters as argument. Define a container and encapsulate them"*


## Rule description
More than six parameters for a method will face this hint. 
It is hard to understand such long list of parameters, which become contradictory and hard to use as they grow longer. 
Instead of a long list of parameters, a method can use the data of an object.

## Example 1
```csharp
public void MyMethod(string UserName, string Password, string Name, string Family, string CompanyPhone, string HomePhone, string Mobile )
{
    //several lines of code
}
```
*should be* 🡻

```csharp
public void MyMethod(RegisterClass register)
{
    //several lines of code
}
public class RegisterClass
    {
        string UserName { get; set; }
        string Password { get; set; }
        string Name { get; set; }
        string Family { get; set; }
        string CompanyPhone { get; set; }
        string HomePhone { get; set; }
        string Mobile { get; set; }
       
    }
```