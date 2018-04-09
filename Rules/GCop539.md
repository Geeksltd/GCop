# GCop 539

> *"Don't hard-code URLs in the code as they might be subject to change. Use Config.Get(...) instead."*

## Rule description

By their nature, Urls can change over time. Also there might be different versions of a URL (e.g. one for testing, one for live).

When you use a URL in your application, you may have to change it while the application is live. By moving them to the config file you can make changes without having to recompile the whole program. 

## Example

```csharp
var myLink = $"https://telegram.me/{botName}";
```

*should be* 🡻

```csharp
var telegramLink = Config.Get("TelegramUrl");
var myLink = $"{telegramLink}{botName}";
```