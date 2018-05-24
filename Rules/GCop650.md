# GCop 650

> *"If this is an Api controller, instead of [Authorize] use [AuthorizeApi] attribute."*

## Rule description

The standard `[Authorize]` attribute in ASP.NET MVC will check the current HTTP user's roles using HttpRequest *Cookies*. That's designed for normal users (browser sessions).

But Web Apis calls coming from server applications don't use cookies. Instead they use Http Headers which makes the standard `[Authorize]` attribute useless. In Olive, there is another class called `[AuthorizeApi]` which will do the same job as the normal `[Authorize]` but via Http Headers instead of Cookies.

## Example

```csharp
[Authorize("SomeRole")]
public class ApiController : BaseController
{
    ...
}
```

*should be* 🡻

```csharp
[AuthorizeApi("SomeRole")]
public class ApiController : BaseController
{
    ...
}
```
