# GCop539

> *"Don't hard-code URLs in the code as they might be subject to change. Use Config.Get(...) instead."*


## Rule description
The main benefit is indeed to have all Urls in one place and you only need to change it there to update the whole program. 
If different parts of your program use the same string and one instance gets updated, but another not, then you have a problem.

## Example 1
```csharp
var myLink = $"https://telegram.me/{botName}";
```
*should be* 🡻

```csharp
var telegramLink = ConfigurationManager.AppSettings["TelegramUrl"];
var myLink = $"{telegramLink}{botName}";
```

