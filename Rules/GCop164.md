# GCop164

> *"Instead use Config.GetConnectionString()"*
> *"Instead use Config.Get()"*


## Rule description
...

## Example 1
```csharp
public static string BotUrl => ConfigurationManager.AppSettings["BotUrl"];
```
*should be* 🡻

```csharp
public static string BotUrl => Config.Get("BotUrl");

```
 
## Example 2
```csharp
connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionName"].ConnectionString);
```
*should be* 🡻

```csharp
connection = new SqlConnection(Config.GetConnectionString("connectionName"));
```


