# GCop 646

> *"Email addresses should not be hard-coded. Move this to Settings table or Config file."*

## Rule description

By their nature, Emails can change over time. Also there might be different versions of a Email (e.g. one for testing, one for live).

When you use a Email in your application, you may have to change it while the application is live. By moving them to the config file you can make changes without having to recompile the whole program.

## Example

```csharp
var infoEmail = "info@companyname.com";
```

*should be* 🡻

```csharp
var infoEmail = Config.Get("infoEmail");
```