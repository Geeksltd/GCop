# GCop179

> "Do not hardcode numbers, strings or other values.
> 
> Declare constant in the top of the file and use it in your code if the value is unlikely to ever change. 
> 
> If the value may change in the future (or on different servers) and the nature of the value is technical, then it must be stored in Web.Config.
> 
> If the nature of the value is business related, or easy to understand by a normal business admin user, then it must be stored in the application in an entity named Setting."


## Rule description
One of the hard-coding issues is that changing the configuration information requires a code change, and then a redeployment of the system. 

Another issue is if the configuration is hard-coded into multiple places within the system, then the programmer must remember all the places the hard-coding exists.

To get around hard-coding configuration information into the system, we often move configuration to an external source or at least declare constants in top of the file.

## Example 1
```csharp
public static void MyMethod()
{
    try
    {
        ...
    }
    catch (Exception)
    {
        throw new HttpException(404, "File Not Found");    
    }
}
```
*should be* 🡻

```csharp
public static void MyMethod()
{
    try
    {
        ...
    }
    catch (Exception)
    {
        //use code below if it is WebApi project
        throw new HttpResponseException(HttpStatusCode.NotFound);
        //use code below if it is not a WebApi project
        throw new HttpException((int)HttpStatusCode.NotFound,"");
    }
}
```

## Example 2
```csharp
public static void MyMethod(int maxWidth = 300)
{ 
    ...    
}
```
*should be* 🡻

```csharp
public const int constMaxWidth = 300;
public static void MyMethod(int maxWidth = constMaxWidth)
{ 
    ...   
}
```

## Example 3
```csharp
var result = myList.Where(lai => lai.ClassID == 18).ToList();
```
*should be* 🡻

```csharp
public class BusinessParameters
{
    public static int MyLimitation
    {
        get { return Settings.Default.MyLimitation; }
    }
}
var result = myList.Where(lai => lai.ClassID == BusinessParameters.MyLimitation).ToList();
```