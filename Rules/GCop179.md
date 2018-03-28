# GCop179

> "Do not hardcode numbers, strings or other values. Use constant fields, config files or database as appropriate."

## Rule description
Hard-coded values in the middle of the code is usually wrong. If the value is hard-coded in multiple places within the system, there is a risk that it can change in the future in one place but not all, and create bugs and inconsistencies.

#### Is the value unlikely to ever change?
In that case declare a constant field at the top of the class and use it instead of hard-coding the value. This way the name of the constant field would be effectively documenting the meaning of that value.

#### Is the value an option from a group?
If the value represents an option from a finite group of other options, then define an Enum for it.

#### Can the value ever change - by developers?
If the value may change in the future (or on different servers) and the nature of the value is technical, then it must be stored in the config file such as *web.config* or *app.config* or *appSettings.json*. This make it possible to change the value on a deployed application without having to recompile the whole application.

#### Can the value ever change - by user?
If the nature of the value is business related, or easy to understand by a normal business admin user, then it must be stored in the application in an entity named Setting or some other appropriate table. This way you can build a secure admin UI for changing it.

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
