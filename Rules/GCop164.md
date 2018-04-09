# GCop 164

> *"Instead use Config.GetConnectionString()"*

> *"Instead use Config.Get()"*

## Rule description

The Config utility class in M#, Zebble and Olive provides a shortcut to reading config values.

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
var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionName"].ConnectionString);
```

*should be* 🡻

```csharp
var connection = new SqlConnection(Config.GetConnectionString("connectionName"));
```

